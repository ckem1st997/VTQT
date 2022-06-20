using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Domain.Common;
using VTQT.Core.Infrastructure;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Helpers;
using VTQT.Web.Framework.Modelling;
using VTQT.Web.Framework.Settings;
using VTQT.Web.Framework.UI;

namespace VTQT.Web.Framework.Extensions
{
    /// <summary>
    /// HTML extensions
    /// </summary>
    public static class HtmlExtensions
    {
        public static IHtmlContent Hint(this IHtmlHelper helper, string value)
        {
            // create a
            var a = new TagBuilder("a");
            a.MergeAttribute("href", "#");
            a.MergeAttribute("onclick", "return false;");
            //a.MergeAttribute("rel", "tooltip");
            a.MergeAttribute("title", value);
            a.MergeAttribute("tabindex", "-1");
            a.AddCssClass("hint");

            // Create img
            var img = new TagBuilder("i");
            img.AddCssClass("fa fa-question-circle");

            a.InnerHtml.AppendHtml(img);

            // Render tag
            return new HtmlString(a.ToString());
        }

        public static IHtmlContent LocalizedEditor<T, TLocalizedModelLocal>(this IHtmlHelper<T> helper, string name,
             Func<int, HelperResult> localizedTemplate,
             Func<T, HelperResult> standardTemplate)
            where T : ILocalizedMvcModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedMvcLocaleModel
        {
            // Localized Editor using XBase TabStrip
            return new HelperResult(writer =>
            {
                if (helper.ViewData.Model.Locales.Count > 1)
                {
                    var languageService = EngineContext.Current.Resolve<ILanguageService>();
                    var appContext = EngineContext.Current.Resolve<IAppContext>();
                    var app = appContext.CurrentApp;
                    var flagsUri = PathHelper.Images.Flags.RootPath.CombineBaseUri(app.CdnUrl);

                    writer.Write("<div class='well well-sm'>");
                    var tabStrip = helper.XBase().TabStrip().Name(name).XBaseTabSelection(false).Style(TabsStyle.Pills).Items(x =>
                    {
                        x.Add().Text("Standard").Content(standardTemplate(helper.ViewData.Model).RenderHtmlContent()).Selected(true);
                        for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                        {
                            var locale = helper.ViewData.Model.Locales[i];
                            var language = languageService.GetLanguageById(locale.LanguageId);
                            x.Add().Text(language.Name)
                                .Content(localizedTemplate(i).RenderHtmlContent())
                                .ImageUrl(flagsUri + language.FlagImageFileName);
                        }
                    }).ToHtmlString();
                    writer.Write(tabStrip);
                    writer.Write("</div>");
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer, HtmlEncoder.Default);
                }

                return Task.CompletedTask;
            });
        }

        #region Custom: XBase Label with required

        // TODO-XBase: Retry or remove
        //public static IHtmlContent XBaseLabel(this IHtmlHelper helper, string expression, string labelText, string hint = null, object htmlAttributes = null, bool required = false)
        //{
        //    var result = new StringBuilder();

        //    var label = helper.Label(expression, labelText, htmlAttributes);
        //    var nodeLabel = HtmlNode.CreateNode(label.RenderHtmlContent());

        //    ////var modelExpressionProdiver = EngineContext.Current.Resolve<ModelExpressionProvider>();
        //    var modelExpressionProdiver = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
        //    modelExpressionProdiver.CreateModelExpression()

        //    if (!nodeLabel.Attributes.Contains("title"))
        //        nodeLabel.SetAttributeValue("title", labelText);

        //    var metadata = ModelMetadata.FromStringExpression(expression, helper.ViewData);
        //    if (!metadata.IsNullableValueType && metadata.ModelType != typeof(Boolean))
        //    {
        //        var validators = metadata.GetValidators(helper.ViewContext.Controller.ControllerContext);
        //        var isRequired = required || validators.Any(a => a.IsRequired);
        //        if (isRequired)
        //        {
        //            var nodeRequired = HtmlNode.CreateNode("<span class=\"required\" aria-required=\"true\">*</span>");
        //            nodeLabel.AppendChild(nodeRequired);
        //        }
        //    }

        //    result.Append(nodeLabel.OuterHtml);

        //    if (hint.HasValue())
        //    {
        //        result.Append(helper.Hint(hint).RenderHtmlContent());
        //    }

        //    return new HtmlString(result.ToString());
        //}

        public static IHtmlContent XBaseLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            bool displayHint = true,
            object htmlAttributes = null,
            bool required = false)
        {
            var modelExpressionProdiver = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var modelExpression = modelExpressionProdiver.CreateModelExpression(helper.ViewData, expression);
            var metadata = modelExpression.Metadata;
            object resourceDisplayName = null;
            metadata.AdditionalValues.TryGetValue("XBaseResourceDisplayName", out resourceDisplayName);

            return XBaseLabelFor(helper, expression, resourceDisplayName as XBaseResourceDisplayName, metadata, displayHint, htmlAttributes, required);
        }

        public static IHtmlContent XBaseLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string resourceKey,
            bool displayHint = true,
            object htmlAttributes = null)
        {
            Guard.ArgumentNotEmpty(() => resourceKey);

            var modelExpressionProdiver = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var modelExpression = modelExpressionProdiver.CreateModelExpression(helper.ViewData, expression);
            var metadata = modelExpression.Metadata;
            var resourceDisplayName = new XBaseResourceDisplayName(resourceKey, metadata.PropertyName);

            return XBaseLabelFor(helper, expression, resourceDisplayName, metadata, displayHint, htmlAttributes);
        }

        private static IHtmlContent XBaseLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            XBaseResourceDisplayName resourceDisplayName,
            ModelMetadata metadata,
            bool displayHint = true,
            object htmlAttributes = null,
            bool required = false)
        {
            var result = new StringBuilder();
            string labelText = null;
            string hint = null;

            if (resourceDisplayName != null)
            {
                // resolve label display name
                labelText = resourceDisplayName.DisplayName.NullEmpty();
                if (labelText == null)
                {
                    // take reskey as absolute fallback
                    labelText = resourceDisplayName.ResourceKey;
                }

                // resolve hint
                if (displayHint)
                {
                    var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    hint = EngineContext.Current.Resolve<ILocalizationService>().GetResource(resourceDisplayName.ResourceKey + ".Hint", langId, true, "", true);
                }
            }

            if (labelText == null)
            {
                labelText = metadata.PropertyName.SplitPascalCase();
            }

            var label = helper.LabelFor(expression, labelText, htmlAttributes);
            var nodeLabel = HtmlNode.CreateNode(label.RenderHtmlContent());

            if (!nodeLabel.Attributes.Contains("title"))
                nodeLabel.SetAttributeValue("title", labelText);

            if (!metadata.IsNullableValueType && metadata.ModelType != typeof(Boolean))
            {
                if (required)
                {
                    var nodeRequired = HtmlNode.CreateNode("<span class=\"required\" aria-required=\"true\">*</span>");
                    nodeLabel.AppendChild(nodeRequired);
                }
            }

            result.Append(nodeLabel.OuterHtml);

            if (displayHint)
            {
                if (hint.HasValue())
                {
                    result.Append(helper.Hint(hint).RenderHtmlContent());
                }
            }

            return new HtmlString(result.ToString());
        }

        #endregion

        #region Custom: Display Label

        public static IHtmlContent XBaseDisplayLabel(this IHtmlHelper helper, string expression, string labelText, object htmlAttributes = null)
        {
            var result = new StringBuilder();

            var label = helper.Label(expression, labelText, htmlAttributes);
            var nodeLabel = HtmlNode.CreateNode(label.RenderHtmlContent());

            if (!nodeLabel.Attributes.Contains("title"))
                nodeLabel.SetAttributeValue("title", labelText);

            result.Append(nodeLabel.OuterHtml);

            return new HtmlString(result.ToString());
        }

        public static IHtmlContent XBaseDisplayLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            object htmlAttributes = null)
        {
            var modelExpressionProdiver = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var modelExpression = modelExpressionProdiver.CreateModelExpression(helper.ViewData, expression);
            var metadata = modelExpression.Metadata;
            object resourceDisplayName = null;
            metadata.AdditionalValues.TryGetValue("XBaseResourceDisplayName", out resourceDisplayName);

            return XBaseDisplayLabelFor(helper, expression, resourceDisplayName as XBaseResourceDisplayName, metadata, htmlAttributes);
        }

        public static IHtmlContent XBaseDisplayLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string resourceKey,
            object htmlAttributes = null)
        {
            Guard.ArgumentNotEmpty(() => resourceKey);

            var modelExpressionProdiver = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var modelExpression = modelExpressionProdiver.CreateModelExpression(helper.ViewData, expression);
            var metadata = modelExpression.Metadata;
            var resourceDisplayName = new XBaseResourceDisplayName(resourceKey, metadata.PropertyName);

            return XBaseDisplayLabelFor(helper, expression, resourceDisplayName, metadata, htmlAttributes);
        }

        private static IHtmlContent XBaseDisplayLabelFor<TModel, TValue>(
            this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            XBaseResourceDisplayName resourceDisplayName,
            ModelMetadata metadata,
            object htmlAttributes = null)
        {
            var result = new StringBuilder();
            string labelText = null;

            if (resourceDisplayName != null)
            {
                // resolve label display name
                labelText = resourceDisplayName.DisplayName.NullEmpty();
                if (labelText == null)
                {
                    // take reskey as absolute fallback
                    labelText = resourceDisplayName.ResourceKey;
                }
            }

            if (labelText == null)
            {
                labelText = metadata.PropertyName.SplitPascalCase();
            }

            var label = helper.LabelFor(expression, labelText, htmlAttributes);
            var nodeLabel = HtmlNode.CreateNode(label.RenderHtmlContent());

            if (!nodeLabel.Attributes.Contains("title"))
                nodeLabel.SetAttributeValue("title", labelText);

            result.Append(nodeLabel.OuterHtml);

            return new HtmlString(result.ToString());
        }

        #endregion

        public static string FieldNameFor<T, TResult>(this IHtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var modelExpressionProdiver = html.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(modelExpressionProdiver.GetExpressionText(expression));
        }

        public static string FieldIdFor<T, TResult>(this IHtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var modelExpressionProdiver = html.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var id = html.GenerateIdFromName(html.ViewData.TemplateInfo.GetFullHtmlFieldName(modelExpressionProdiver.GetExpressionText(expression)));
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
        }

        #region DropDownList Extensions

        private static readonly SelectListItem[] _singleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        public static IHtmlContent DropDownListForEnum<TModel, TEnum>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            string optionLabel = null) where TEnum : struct
        {

            return htmlHelper.DropDownListForEnum(expression, null, optionLabel);
        }

        public static IHtmlContent DropDownListForEnum<TModel, TEnum>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            object htmlAttributes,
            string optionLabel = null) where TEnum : struct
        {
            IDictionary<string, object> attrs = null;
            if (htmlAttributes != null)
            {
                attrs = CommonHelper.ObjectToDictionary(htmlAttributes);
            }

            return htmlHelper.DropDownListForEnum(expression, attrs, optionLabel);
        }

        public static IHtmlContent DropDownListForEnum<TModel, TEnum>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            IDictionary<string, object> htmlAttributes,
            string optionLabel = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("An Enumeration type is required.", "expression");

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var modelExpressionProdiver = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
            var modelExpression = modelExpressionProdiver.CreateModelExpression(htmlHelper.ViewData, expression);
            var metadata = modelExpression.Metadata;
            Type enumType = metadata.ModelType.GetNonNullableType();
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            var items = Enumerable.Empty<SelectListItem>();
            foreach (var value in values)
            {
                items.Append(new SelectListItem
                {
                    Text = value.GetLocalizedEnum(localizationService, workContext),
                    Value = Enum.GetName(enumType, value),
                    Selected = value.Equals(modelExpression.Model.Convert(enumType))
                });
            }

            if (metadata.IsNullableValueType)
            {
                items = _singleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(expression, items, optionLabel, htmlAttributes);
        }

        #endregion

        public static IHtmlContent MetaAcceptLanguage(this IHtmlHelper html)
        {
            var acceptLang = HttpUtility.HtmlAttributeEncode(Thread.CurrentThread.CurrentUICulture.ToString());
            return new HtmlString(string.Format("<meta name=\"accept-language\" content=\"{0}\"/>", acceptLang));
        }

        public static IHtmlContent TableFormattedVariantAttributes(this IHtmlHelper helper, string formattedVariantAttributes, string separatorLines = "<br />", string separatorValues = ": ")
        {
            var sb = new StringBuilder();
            string name, value;
            string[] lines = formattedVariantAttributes.SplitSafe(separatorLines);

            if (lines.Length <= 0)
                return HtmlString.Empty;

            sb.Append("<table class=\"product-attribute-table\">");

            foreach (string line in lines)
            {
                sb.Append("<tr>");
                if (line.SplitToPair(out name, out value, separatorValues))
                {
                    sb.AppendFormat("<td class=\"column-name\">{0}:</td>", name);
                    sb.AppendFormat("<td class=\"column-value\">{0}</td>", value);
                }
                else
                {
                    sb.AppendFormat("<td colspan=\"2\">{0}</td>", line);
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent SettingOverrideCheckbox<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string parentSelector = null)
        {
            var data = helper.ViewData[AppDependingSettingHelper.ViewDataKey] as AppDependingSettingData;

            if (data != null && data.ActiveAppScopeConfiguration != SettingDefaults.AllAppsId)
            {
                var modelExpressionProdiver = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
                var settingKey = modelExpressionProdiver.GetExpressionText(expression);
                var localizeService = EngineContext.Current.Resolve<ILocalizationService>();

                if (!settingKey.Contains("."))
                    settingKey = data.RootSettingClass + "." + settingKey;

                var overrideForApp = (data.OverrideSettingKeys.FirstOrDefault(x => x.IsCaseInsensitiveEqual(settingKey)) != null);
                var fieldId = settingKey + (settingKey.EndsWith("_OverrideForApp") ? "" : "_OverrideForApp");

                var sb = new StringBuilder();
                sb.Append("<div class=\"onoffswitch-container\"><div class=\"onoffswitch\">");

                sb.AppendFormat("<input type=\"checkbox\" id=\"{0}\" name=\"{0}\" class=\"onoffswitch-checkbox multi-app-override-option\"", fieldId);
                sb.AppendFormat(" onclick=\"admin.apps.overriddenAppSetting.checkValue(this)\" data-parent-selector=\"{0}\"{1} />", parentSelector.EmptyNull(), overrideForApp ? " checked=\"checked\"" : "");

                sb.AppendFormat("<label class=\"onoffswitch-label\" for=\"{0}\">", fieldId);
                sb.AppendFormat("<span class=\"onoffswitch-on\">{0}</span>", localizeService.GetResource("Common.On").Truncate(3).ToUpper());
                sb.AppendFormat("<span class=\"onoffswitch-off\">{0}</span>", localizeService.GetResource("Common.Off").Truncate(3).ToUpper());
                sb.Append("<span class=\"onoffswitch-switch\"></span>");
                sb.Append("<span class=\"onoffswitch-inner\"></span>");
                sb.Append("</label>");
                sb.Append("</div></div>\r\n");      // controls are not floating, so line-break prevents different distances between them

                // Using Bootstrap Switch with input (checkbox, radio) has class "make-switch".
                // But it will show checkbox for a short time before show Bootstrap Switch
                //var sb = new StringBuilder();
                //sb.AppendFormat("<input type=\"checkbox\" id=\"{0}\" name=\"{0}\" class=\"make-switch multi-app-override-option\" data-size=\"mini\"", fieldId);
                //sb.AppendFormat(" data-parent-selector=\"{0}\"{1} />", parentSelector.EmptyNull(), overrideForApp ? " checked=\"checked\"" : "");

                return new HtmlString(sb.ToString());
            }
            return HtmlString.Empty;
        }

        public static IHtmlContent SettingEditorFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string parentSelector = null,
            object additionalViewData = null)
        {
            var checkbox = helper.SettingOverrideCheckbox(expression, parentSelector);
            var editor = helper.EditorFor(expression, additionalViewData);

            return new HtmlString(checkbox.RenderHtmlContent() + editor.ToString());
        }

        public static IHtmlContent CollapsedText(this IHtmlHelper helper, string text)
        {
            if (text.IsEmpty())
                return HtmlString.Empty;

            var catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();

            if (!catalogSettings.EnableHtmlTextCollapser)
                return new HtmlString(text);

            string options = "{{\"adjustheight\":{0}}}".FormatWith(
                catalogSettings.HtmlTextCollapsedHeight
            );

            string result = "<div class='more-less' data-options='{0}'><div class='more-block'>{1}</div></div>".FormatWith(
                options, text
            );

            return new HtmlString(result);
        }

        public static IHtmlContent IconForFileExtension(this IHtmlHelper helper, string fileExtension, bool renderLabel = false)
        {
            return IconForFileExtension(helper, fileExtension, null, renderLabel);
        }

        public static IHtmlContent IconForFileExtension(this IHtmlHelper helper, string fileExtension, string extraCssClasses = null, bool renderLabel = false)
        {
            Guard.ArgumentNotNull(() => helper);
            Guard.ArgumentNotEmpty(() => fileExtension);

            var icon = "file-o";
            var ext = fileExtension;

            if (ext != null && ext.StartsWith("."))
            {
                ext = ext.Substring(1);
            }

            if (ext.HasValue())
            {
                switch (ext.ToLowerInvariant())
                {
                    case "pdf":
                        icon = "file-pdf-o";
                        break;
                    case "doc":
                    case "docx":
                    case "docm":
                    case "odt":
                    case "dot":
                    case "dotx":
                    case "dotm":
                        icon = "file-word-o";
                        break;
                    case "xls":
                    case "xlsx":
                    case "xlsm":
                    case "xlsb":
                    case "ods":
                        icon = "file-excel-o";
                        break;
                    case "csv":
                    case "tab":
                        icon = "table";
                        break;
                    case "ppt":
                    case "pptx":
                    case "pptm":
                    case "ppsx":
                    case "odp":
                    case "potx":
                    case "pot":
                    case "potm":
                    case "pps":
                    case "ppsm":
                        icon = "file-powerpoint-o";
                        break;
                    case "zip":
                    case "rar":
                    case "7z":
                        icon = "file-archive-o";
                        break;
                    case "png":
                    case "jpg":
                    case "jpeg":
                    case "bmp":
                    case "psd":
                        icon = "file-image-o";
                        break;
                    case "mp3":
                    case "wav":
                    case "ogg":
                    case "wma":
                        icon = "file-audio-o";
                        break;
                    case "mp4":
                    case "mkv":
                    case "wmv":
                    case "avi":
                    case "asf":
                    case "mpg":
                    case "mpeg":
                        icon = "file-video-o";
                        break;
                    case "txt":
                        icon = "file-text-o";
                        break;
                    case "exe":
                        icon = "gear";
                        break;
                    case "xml":
                    case "html":
                    case "htm":
                        icon = "file-code-o";
                        break;
                }
            }

            var label = ext.NaIfEmpty().ToUpper();

            var result = "<i class='fa fa-fw fa-{0}{1}' title='{2}'></i>".FormatInvariant(
                icon,
                extraCssClasses.HasValue() ? " " + extraCssClasses : "",
                label);

            if (renderLabel)
            {
                if (ext.IsEmpty())
                {
                    result = "<span class='muted'>{0}</span>".FormatInvariant("".NaIfEmpty());
                }
                else
                {
                    result = result + "<span class='ml4'>{0}</span>".FormatInvariant(label);
                }
            }

            return new HtmlString(result);
        }

        // TODO-XBase: Retry or remove
        //public static IDictionary<string, object> UnobtrusiveValidationAttributesFor<TModel, TProperty>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> propertyExpression)
        //{
        //    var propertyName = html.NameFor(propertyExpression).ToString();
        //    ////var metadata = ModelMetadata.FromLambdaExpression(propertyExpression, html.ViewData);
        //    //var metadata = ModelMetadata.FromStringExpression(propertyName, html.ViewData);
        //    var modelExpressionProdiver = html.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>();
        //    var modelExpression = modelExpressionProdiver.CreateModelExpression(html.ViewData, propertyName);

        //    var attributes = html.GetUnobtrusiveValidationAttributes(propertyName, modelExpression.Metadata);
        //    return attributes;
        //}

        //public static string UnobtrusiveHtmlValidationAttributesFor<TModel, TProperty>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> propertyExpression)
        //{
        //    var attributes = html.UnobtrusiveValidationAttributesFor(propertyExpression);

        //    var htmlAttributes = new StringBuilder();
        //    foreach (var attribute in attributes)
        //    {
        //        htmlAttributes.Append($"{attribute.Key}=\"{attribute.Value}\" ");
        //    }
        //    return htmlAttributes.ToString();
        //}

        #region Common extensions

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="htmlContent">HTML content</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public static async Task<string> RenderHtmlContentAsync(this IHtmlContent htmlContent)
        {
            await using var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="htmlContent">HTML content</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public static string RenderHtmlContent(this IHtmlContent htmlContent)
        {
            using var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        #endregion
    }
}
