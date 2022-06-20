using System.Runtime.CompilerServices;
using VTQT.Core;
using VTQT.Core.Infrastructure;
using VTQT.Services.Localization;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Web.Framework
{
    // Dùng cho flow Mvc
    /// <summary>
    /// Represents model attribute that specifies the display name by passed key of the locale resource
    /// </summary>
    public class XBaseResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private readonly string _callerPropertyName;

        public XBaseResourceDisplayName(string resourceKey, [CallerMemberName] string propertyName = null)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _callerPropertyName = propertyName;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                string value = null;
                var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                value = EngineContext.Current.Resolve<ILocalizationService>().GetResource(ResourceKey, langId, true, "" /* ResourceKey */, true);

                if (value.IsEmpty() && _callerPropertyName.HasValue())
                {
                    value = _callerPropertyName.SplitPascalCase();
                }

                return value;
            }
        }

        public string Name => nameof(XBaseResourceDisplayName);
    }
}
