using System.Collections.Generic;
using VTQT.Core;
using VTQT.Core.Configuration;
using VTQT.Elastic.Documents.Common;

namespace VTQT.Elastic.Helpers
{
    public class ElasticIndexHelper
    {
        public static readonly int LimitTotalFields = 2000; // Default: 1000

        public static readonly string IndexNameFormat = "{0}_{1}";

        private static string EnvironmentName;
        private static Dictionary<string, string> IndexNames = new Dictionary<string, string>();

        public static string GetIndexName<T>()
            where T : class, IElasticDoc
        {
            var type = typeof(T);
            var typeName = type.Name;
            var result = string.Empty;

            if (!IndexNames.TryGetValue(typeName, out result))
            {
                if (string.IsNullOrWhiteSpace(EnvironmentName))
                    EnvironmentName = CommonHelper.GetAppSetting<string>(
                        $"{ElasticConfig.Elastic}:{nameof(ElasticConfig.EnvironmentName)}");

                #region Common
                if (type == typeof(LogDoc))
                {
                    result = string.Format(IndexNameFormat, EnvironmentName, "log");
                }
                #endregion

                IndexNames.Add(typeName, result);
            }

            return result;
        }


    }
}
