using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Infrastructure;
using VTQT.Services.Localization;

namespace VTQT.Services.Helpers
{
    public static class EnumHelper
    {
        #region ListItem - XBase

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="selectedValue">Selected value</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static async Task<IList<XBaseListItem>> GetXBaseListItemsAsync<TEnum>(
            int? selectedValue = null, int[] valuesToExclude = null, bool useLocalization = true)
            where TEnum : struct
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var values = await Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                    valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
                .SelectAwait(async enumValue => new
                {
                    Id = Convert.ToInt32(enumValue),
                    Name = useLocalization
                        ? await localizationService.GetLocalizedEnumAsync(enumValue, workContext.LanguageId)
                        : CommonHelper.ConvertEnum(enumValue.ToString())
                }).ToListAsync();

            var result = values.Select(s => new XBaseListItem
            {
                Id = s.Id,
                Name = s.Name,
                Selected = selectedValue.HasValue && s.Id == selectedValue.Value
            }).ToList();

            return result;
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="selectedValue">Selected value</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static IList<XBaseListItem> GetXBaseListItems<TEnum>(
            int? selectedValue = null, int[] valuesToExclude = null, bool useLocalization = true)
            where TEnum : struct
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                    valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
                .Select(enumValue => new
                {
                    Id = Convert.ToInt32(enumValue),
                    Name = useLocalization
                        ? localizationService.GetLocalizedEnum(enumValue, workContext.LanguageId)
                        : CommonHelper.ConvertEnum(enumValue.ToString())
                }).ToList();

            var result = values.Select(s => new XBaseListItem
            {
                Id = s.Id,
                Name = s.Name,
                Selected = selectedValue.HasValue && s.Id == selectedValue.Value
            }).ToList();

            return result;
        }

        #endregion

        #region ListItem - MVC

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="selectedValue">Selected value</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static async Task<IList<SelectListItem>> GetMvcListItemsAsync<TEnum>(
            int? selectedValue = null, int[] valuesToExclude = null, bool useLocalization = true)
            where TEnum : struct
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var values = await Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                    valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
                .SelectAwait(async enumValue => new
                {
                    Id = Convert.ToInt32(enumValue),
                    Name = useLocalization
                        ? await localizationService.GetLocalizedEnumAsync(enumValue, workContext.LanguageId)
                        : CommonHelper.ConvertEnum(enumValue.ToString())
                }).ToListAsync();

            var result = values.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = selectedValue.HasValue && s.Id == selectedValue.Value
            }).ToList();

            return result;
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="selectedValue">Selected value</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static IList<SelectListItem> GetMvcListItems<TEnum>(
            int? selectedValue = null, int[] valuesToExclude = null, bool useLocalization = true)
            where TEnum : struct
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var values = Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                    valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
                .Select(enumValue => new
                {
                    Id = Convert.ToInt32(enumValue),
                    Name = useLocalization
                        ? localizationService.GetLocalizedEnum(enumValue, workContext.LanguageId)
                        : CommonHelper.ConvertEnum(enumValue.ToString())
                }).ToList();

            var result = values.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = selectedValue.HasValue && s.Id == selectedValue.Value
            }).ToList();

            return result;
        }

        #endregion
    }
}
