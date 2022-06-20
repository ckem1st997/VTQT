namespace VTQT.Web.Framework.Security
{
    public static class AntiForgeryConfig
    {
        public const string TokenFieldName = "__RequestVerificationToken";
        public const string HeaderName = "RequestVerificationToken";
    }
}
