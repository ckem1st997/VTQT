using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace VTQT.ComponentModel
{
    public class XBaseContractResolver : DefaultContractResolver
    {
        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            if (member is PropertyInfo pi)
            {
                return new FastPropertyValueProvider(pi);
            }

            return base.CreateMemberValueProvider(member);
        }

        public static XBaseContractResolver Instance { get; } = new XBaseContractResolver();
    }
}
