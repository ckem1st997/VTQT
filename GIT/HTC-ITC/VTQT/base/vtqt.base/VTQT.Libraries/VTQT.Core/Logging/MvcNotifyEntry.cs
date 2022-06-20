using System;
using VTQT.Core.Localization;

namespace VTQT.Core.Logging
{

    public enum MvcNotifyType
    {
        Info,
        Success,
        Warning,
        Error
    }

    [Serializable]
    public class MvcNotifyEntry : ComparableObject<MvcNotifyEntry>
    {
        [ObjectSignature]
        public MvcNotifyType Type { get; set; }

        [ObjectSignature]
        public LocalizedString Message { get; set; }

        public bool Durable { get; set; }
    }

}
