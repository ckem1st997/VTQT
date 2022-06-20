using System;
using System.ComponentModel;

namespace VTQT.SharedMvc.Master.Extensions
{
    /// <summary>
    /// Lớp mở rộng phương thức Enum
    /// </summary>
    public static class EnumExtensionMethods
    {
        /// <summary>
        /// Hàm trả về Description Enum
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
    }
}
