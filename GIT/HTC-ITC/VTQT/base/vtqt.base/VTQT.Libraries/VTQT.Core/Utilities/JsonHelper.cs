using Newtonsoft.Json;
using VTQT.ComponentModel;

namespace VTQT.Utilities
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = XBaseContractResolver.Instance,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            //DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Formatting = Formatting.None,

            // We cannot ignore null. Client template of several Telerik grids would fail.
            // NullValueHandling = NullValueHandling.Ignore,

            // Limit the object graph we'll consume to a fixed depth. This prevents stackoverflow exceptions
            // from deserialization errors that might occur from deeply nested objects.
            MaxDepth = 32
        };
    }
}
