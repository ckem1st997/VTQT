using System.IO;
using Microsoft.AspNetCore.Http;

namespace VTQT.Web.Dashboard.Models
{
   public static class FormFileExtensions
    {
        public const int ImageMinimumBytes = 512;

        /// <summary>
        /// Check Image
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static bool IsImage(this IFormFile postedFile)
        {
            /// check image
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check Word
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static bool IsWord(this IFormFile postedFile)
        {
            /// check word
            if (postedFile.ContentType.ToLower() != "application/msword" &&
            postedFile.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
            postedFile.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" &&
            postedFile.ContentType.ToLower() != "application/vnd.ms-word.document.macroEnabled.12" &&
            postedFile.ContentType.ToLower() != "application/vnd.ms-word.template.macroEnabled.12")
            {
                return false;
            }

            if (Path.GetExtension(postedFile.FileName).ToLower() != ".doc"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".dot"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".docx"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".dotx"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".docm"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".dotm")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check Excel
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static bool IsExcel(this IFormFile postedFile)
        {
            /// check excel
            if (postedFile.ContentType.ToLower() != "application/vnd.ms-excel" &&
            postedFile.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
            postedFile.ContentType.ToLower() != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" &&
            postedFile.ContentType.ToLower() != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
            postedFile.ContentType.ToLower() != "application/vnd.ms-excel.addin.macroEnabled.12" &&
            postedFile.ContentType.ToLower() != "application/vnd.ms-excel.sheet.binary.macroEnabled.12" &&
            postedFile.ContentType.ToLower() != "application/vnd.ms-excel.template.macroEnabled.12")
            {
                return false;
            }

            if (Path.GetExtension(postedFile.FileName).ToLower() != ".xls"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xlt"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xla"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xlsx"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xltx"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xlsm"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xltm"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xlam"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xlsb"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xlsm"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".xll"
                )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check Zip and Rar
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static bool IsZipRar(this IFormFile postedFile)
        {
            /// check zip and rar
            if (postedFile.ContentType.ToLower() != "application/x-rar-compressed" &&
            postedFile.ContentType.ToLower() != "application/octet-stream" &&
            postedFile.ContentType.ToLower() != "application/zip" &&
            postedFile.ContentType.ToLower() != "application/octet-stream" &&
            postedFile.ContentType.ToLower() != "application/vnd.rar" &&
            postedFile.ContentType.ToLower() != "application/x-zip-compressed" &&
            postedFile.ContentType.ToLower() != "multipart/x-zip")
            {
                return false;
            }
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".rar"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".zip"
                )
            {
                return false;
            }
            return true;
        }
    }
}
