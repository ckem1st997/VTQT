namespace VTQT.Web.Framework.Mvc.Pjax
{
    public interface IPjax
    {
        bool IsPjaxRequest { get; set; }

        string PjaxVersion { get; set; }
    }
}
