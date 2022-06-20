﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using VTQT.Core.Domain.Localization;
using VTQT.Core.Events;
using VTQT.Services.Localization;
using VTQT.Services.Seo;
using VTQT.Web.Framework.Events;

namespace VTQT.Web.Framework.Routing
{
    /// <summary>
    /// Represents slug route transformer
    /// </summary>
    public class SlugRouteTransformer : DynamicRouteValueTransformer
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public SlugRouteTransformer(IEventPublisher eventPublisher,
            ILanguageService languageService,
            IUrlRecordService urlRecordService,
            LocalizationSettings localizationSettings)
        {
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _urlRecordService = urlRecordService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (values == null)
                return values;

            if (!values.TryGetValue("SeName", out var slugValue) || string.IsNullOrEmpty(slugValue as string))
                return values;

            var slug = slugValue as string;
            var urlRecord = await _urlRecordService.GetBySlugAsync(slug);

            //no URL record found
            if (urlRecord == null)
                return values;

            //virtual directory path
            var pathBase = httpContext.Request.PathBase;

            //if URL record is not active let's find the latest one
            if (!urlRecord.IsActive)
            {
                var activeSlug = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                if (string.IsNullOrEmpty(activeSlug))
                    return values;

                //redirect to active slug if found
                values[XBasePathRouteDefaults.ControllerFieldKey] = "Common";
                values[XBasePathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                values[XBasePathRouteDefaults.UrlFieldKey] = $"{pathBase}/{activeSlug}{httpContext.Request.QueryString}";
                values[XBasePathRouteDefaults.PermanentRedirectFieldKey] = true;
                httpContext.Items["xbase.RedirectFromGenericPathRoute"] = true;

                return values;
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var urllanguage = values["language"];
                if (urllanguage != null && !string.IsNullOrEmpty(urllanguage.ToString()))
                {
                    var languages = await _languageService.GetAllLanguagesAsync();
                    var language = languages.FirstOrDefault(x => x.UniqueSeoCode.ToLowerInvariant() == urllanguage.ToString().ToLowerInvariant())
                        ?? languages.FirstOrDefault();

                    var slugForCurrentLanguage = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                    if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                        //redirect to the page for current language
                        values[XBasePathRouteDefaults.ControllerFieldKey] = "Common";
                        values[XBasePathRouteDefaults.ActionFieldKey] = "InternalRedirect";
                        values[XBasePathRouteDefaults.UrlFieldKey] = $"{pathBase}/{language.UniqueSeoCode}/{slugForCurrentLanguage}{httpContext.Request.QueryString}";
                        values[XBasePathRouteDefaults.PermanentRedirectFieldKey] = false;
                        httpContext.Items["xbase.RedirectFromGenericPathRoute"] = true;

                        return values;
                    }
                }
            }

            //since we are here, all is ok with the slug, so process URL
            switch (urlRecord.EntityName.ToLowerInvariant())
            {
                //case "product":
                //    values[XBasePathRouteDefaults.ControllerFieldKey] = "Product";
                //    values[XBasePathRouteDefaults.ActionFieldKey] = "ProductDetails";
                //    values[XBasePathRouteDefaults.ProductIdFieldKey] = urlRecord.EntityId;
                //    values[XBasePathRouteDefaults.SeNameFieldKey] = urlRecord.Slug;
                //    break;

                

                default:
                    //no record found, thus generate an event this way developers could insert their own types
                    await _eventPublisher.PublishAsync(new GenericRoutingEvent(values, urlRecord));
                    break;
            }

            return values;
        }

        #endregion
    }
}
