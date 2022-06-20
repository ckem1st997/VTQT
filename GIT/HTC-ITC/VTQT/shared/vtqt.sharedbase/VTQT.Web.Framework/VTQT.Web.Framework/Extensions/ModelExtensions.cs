using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VTQT.Core;
using VTQT.Core.Infrastructure;
using VTQT.Web.Framework.Modelling;

namespace VTQT
{
    public static class ModelExtensions
    {
        public static void BindRequest(this BaseSearchModel model, DataSourceRequest request)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            model.PageIndex = request.Page;
            model.PageSize = request.PageSize;
            model.LanguageId = workContext.LanguageId;
        }

        public static string GetErrorsToHtml<T>(this XBaseResult<T> result)
            where T : class
        {
            var errorMessages = result.errors.SelectMany(x => x.Value);

            return GetErrorsToHtml(errorMessages, result.message);
        }

        public static string GetErrorsToHtml(this ModelStateDictionary modelState, string message = null)
        {
            var errorMessages = modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

            return GetErrorsToHtml(errorMessages, message);
        }

        public static string GetErrorsToHtml(this IEnumerable<string> errorMessages, string message = null)
        {
            var sbErrors = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(message))
                sbErrors.Append($"<span>{message}</span>");

            if (errorMessages != null && errorMessages.Any())
            {
                sbErrors.Append("<ul>");
                foreach (var errorMessage in errorMessages)
                {
                    sbErrors.Append($"<li>{errorMessage}</li>");
                }
                sbErrors.Append("</ul>");
            }

            return sbErrors.ToString();
        }

        public static Dictionary<string, IEnumerable<string>> GetErrors(this ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(w => w.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(s => s.ErrorMessage)
                );

            return errors;
        }
    }
}
