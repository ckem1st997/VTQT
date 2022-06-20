using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Domain;
using VTQT.Core.Infrastructure;
using VTQT.Services.Localization;

namespace VTQT.Services
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        #region ListItem - XBase

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumObj">Enum</param>
        /// <param name="markCurrentAsSelected">Mark current value as selected</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static async Task<IList<XBaseListItem>> ToXBaseListItemsAsync<TEnum>(this TEnum enumObj,
            bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true)
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

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            var result = values.Select(s => new XBaseListItem
            {
                Id = s.Id,
                Name = s.Name,
                Selected = selectedValue != null && s.Id == (int)selectedValue
            }).ToList();

            return result;
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumObj">Enum</param>
        /// <param name="markCurrentAsSelected">Mark current value as selected</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static IList<XBaseListItem> ToXBaseListItems<TEnum>(this TEnum enumObj,
            bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true)
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

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            var result = values.Select(s => new XBaseListItem
            {
                Id = s.Id,
                Name = s.Name,
                Selected = selectedValue != null && s.Id == (int)selectedValue
            }).ToList();

            return result;
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="objList">List of objects</param>
        /// <param name="selector">Selector for name</param>
        /// <returns>List of select items</returns>
        public static IList<XBaseListItem> ToXBaseListItems<T>(this T objList, Func<BaseEntity, string> selector)
            where T : IEnumerable<BaseEntity>
        {
            var result = objList.Select(p => new XBaseListItem { Id = p.Id, Name = selector(p) }).ToList();

            return result;
        }

        #endregion

        #region ListItem - MVC

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumObj">Enum</param>
        /// <param name="markCurrentAsSelected">Mark current value as selected</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static async Task<IList<SelectListItem>> ToMvcListItemsAsync<TEnum>(this TEnum enumObj,
            bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true)
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

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            var result = values.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = selectedValue != null && s.Id == (int)selectedValue
            }).ToList();

            return result;
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumObj">Enum</param>
        /// <param name="markCurrentAsSelected">Mark current value as selected</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>List of select items</returns>
        public static IList<SelectListItem> ToMvcListItems<TEnum>(this TEnum enumObj,
            bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true)
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

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            var result = values.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = selectedValue != null && s.Id == (int)selectedValue
            }).ToList();

            return result;
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="objList">List of objects</param>
        /// <param name="selector">Selector for name</param>
        /// <returns>List of select items</returns>
        public static IList<SelectListItem> ToMvcListItems<T>(this T objList, Func<BaseEntity, string> selector)
            where T : IEnumerable<BaseEntity>
        {
            var result = objList.Select(p => new SelectListItem { Value = p.Id, Text = selector(p) }).ToList();

            return result;
        }

        #endregion
    }
}
