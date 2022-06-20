using System;
using System.Reflection;
using System.Text;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Core.Domain;
using VTQT.Core.Helpers;

namespace VTQT.Web.Framework.Helpers
{
    public partial class AppHelper : AppHelperBase
    {
        public static void Init()
        {
            InitAspose();
        }

        #region Third Party

        public static void InitAspose()
        {
            new Aspose.BarCode.License().SetLicense(AsposeHelper.BarCodeLicenseStream);
            new Aspose.Cells.License().SetLicense(AsposeHelper.CellsLicenseStream);
            new Aspose.Pdf.License().SetLicense(AsposeHelper.PdfLicenseStream);
            new Aspose.Words.License().SetLicense(AsposeHelper.WordsLicenseStream);

            // Fix tương thích trên .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #endregion

        public static string GetAppProjectType()
        {
            return CommonHelper.GetAppSetting<string>($"{XBaseConfig.XBase}:{nameof(AppProjectType)}");
        }

        public static class FrameworkAssembly
        {
            public static readonly Assembly Reference = typeof(FrameworkAssembly).Assembly;
            public static readonly Version Version = Reference.GetName().Version;
        }
    }
}
