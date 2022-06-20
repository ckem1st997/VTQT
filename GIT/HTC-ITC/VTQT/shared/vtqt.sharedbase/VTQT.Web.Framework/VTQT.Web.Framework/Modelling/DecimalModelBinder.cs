using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using VTQT.Core.Infrastructure;

namespace VTQT.Web.Framework.Modelling
{
    public class DecimalModelBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;

        public DecimalModelBinder(Type modelType)
        {
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            _baseBinder = new SimpleTypeModelBinder(modelType, loggerFactory);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // https://gist.github.com/mustafadagdelen/d864b935e19510a9c3e40c1f30c2ca83
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;
                decimal result;

                // Use invariant culture
                if (decimal.TryParse(valueAsString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
            return _baseBinder.BindModelAsync(bindingContext);

            //// XBase
            //object result = null;

            //// Don't do this here!
            //// It might do bindingContext.ModelState.AddModelError
            //// and there is no RemoveModelError!
            //// 
            //// result = base.BindModel(controllerContext, bindingContext);

            //string modelName = bindingContext.ModelName;
            //string attemptedValue = bindingContext.ValueProvider.GetValue(modelName).FirstValue;

            //if (!string.IsNullOrWhiteSpace(attemptedValue))
            //{
            //    // Depending on CultureInfo, the NumberDecimalSeparator can be "," or "."
            //    // Both "." and "," should be accepted, but aren't.
            //    string wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            //    string alternateSeperator = (wantedSeperator == "," ? "." : ",");

            //    if (attemptedValue.IndexOf(wantedSeperator) == -1
            //        && attemptedValue.IndexOf(alternateSeperator) != -1)
            //    {
            //        attemptedValue = attemptedValue.Replace(alternateSeperator, wantedSeperator);
            //    }
            //}

            //try
            //{
            //    if (bindingContext.ModelMetadata.IsNullableValueType
            //        && string.IsNullOrWhiteSpace(attemptedValue))
            //    {
            //        return null;
            //    }

            //    result = decimal.Parse(attemptedValue, NumberStyles.Any);

            //    bindingContext.Result = ModelBindingResult.Success(result);
            //    return Task.CompletedTask;
            //}
            //catch (FormatException e)
            //{
            //    bindingContext.ModelState.AddModelError(modelName, e, bindingContext.ModelMetadata);
            //}

            //return _baseBinder.BindModelAsync(bindingContext);
        }
    }
}
