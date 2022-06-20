using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using VTQT.Core;

namespace VTQT.Web.Framework
{
    public static class KendoExtensions
    {
        public static GridTemplateColumnBuilder<T> XBaseSelect<T>(this GridColumnFactory<T> columnFactory) where T : class
        {
            return columnFactory
                .Template("<input type=\"checkbox\" class=\"row-checkbox\" />")
                .ClientHeaderTemplate("<input type=\"checkbox\" class=\"check-all\" />")
                .HeaderHtmlAttributes(new { @class = "check-all" })
                .Width(30).Centered();
        }

        public static GridBoundColumnBuilder<T> Centered<T>(this GridBoundColumnBuilder<T> columnBuilder) where T : class
        {
            return columnBuilder.HtmlAttributes(new { align = "center" }).HeaderHtmlAttributes(new { style = "text-align:center;" });
        }
        public static GridTemplateColumnBuilder<T> Centered<T>(this GridTemplateColumnBuilder<T> columnBuilder) where T : class
        {
            return columnBuilder.HtmlAttributes(new { align = "center" }).HeaderHtmlAttributes(new { style = "text-align:center;" });
        }
        public static GridBoundColumnBuilder<T> RightAlign<T>(this GridBoundColumnBuilder<T> columnBuilder) where T : class
        {
            return columnBuilder.HtmlAttributes(new { style = "text-align:right;" }).HeaderHtmlAttributes(new { style = "text-align:right;" });
        }
        public static GridTemplateColumnBuilder<T> RightAlign<T>(this GridTemplateColumnBuilder<T> columnBuilder) where T : class
        {
            return columnBuilder.HtmlAttributes(new { style = "text-align:right;" }).HeaderHtmlAttributes(new { style = "text-align:right;" });
        }

        #region DataSource

        public static DataSourceResult ToGroupDataSourceResult(this IEnumerable<object> models, IList<GroupDescriptor> groups, int totalCount)
        {
            var result = new DataSourceResult
            {
                AggregateResults = null,
                Data = models.ToGroupDataSource(groups),
                Errors = null,
                Total = totalCount
            };
            return result;
        }

        public static IEnumerable<object> ToGroupDataSource(this IEnumerable<object> models, IList<GroupDescriptor> groups)
        {
            /*
             * Chú ý:
             * - Xác định HasSubgroups (true/false), Subgroups cho đúng,
             * nếu không còn Subgroup mà HasSubgroups=true thì sẽ gặp lỗi ở kendo.all.js:
             * Uncaught TypeError: Cannot read property 'length' of undefined
             */

            if (models == null)
                throw new ArgumentException(nameof(models));
            if (groups == null)
                throw new ArgumentException(nameof(groups));

            if (!models.Any())
                return new List<object>();
            if (!groups.Any())
                return models;

            var grpFields = groups.Select(s => s.Member).ToList();
            var grpField = grpFields.First();

            var grpRoots = models.GroupBy(
                g => (string)g.GetType().GetProperty(grpField).GetValue(g, null),
                (key, g) => new GroupObjectItem
                {
                    Key = key,
                    Data = g
                });

            grpFields.Remove(grpField);

            var subGrpFields = new List<string>(grpFields);

            var resultData = new List<object>();
            foreach (var grpRoot in grpRoots)
            {
                var grpSubItems = subGrpFields.Any()
                    ? SubGroupDataSource(grpRoot, subGrpFields)
                    : grpRoot.Data;

                var item = new
                {
                    Aggregates = new { },
                    HasSubgroups = subGrpFields.Any(),
                    ItemCount = grpRoot.Data.Count(),
                    Items = grpSubItems,
                    Key = grpRoot.Key,
                    Member = grpField,
                    SubgroupCount = 0,
                    Subgroups = subGrpFields.Any() ? grpSubItems : new object[] { }
                };

                resultData.Add(item);
            }

            return resultData;
        }

        private static IEnumerable<object> SubGroupDataSource(GroupObjectItem grpRoot, IList<string> grpFields)
        {
            var tmpGrpFields = new List<string>(grpFields);
            var grpField = tmpGrpFields.First();

            var grpSubs = grpRoot.Data.GroupBy(
                g => (string)g.GetType().GetProperty(grpField).GetValue(g, null),
                (key, g) => new
                {
                    Key = key,
                    Data = g
                });

            tmpGrpFields.Remove(grpField);

            var subGrpFields = new List<string>(tmpGrpFields);

            return new List<object>(grpSubs.Select(s =>
            {
                var grpSubItems = subGrpFields.Any()
                    ? SubGroupDataSource(new GroupObjectItem { Key = s.Key, Data = s.Data }, subGrpFields)
                    : s.Data;
                var item = new
                {
                    Aggregates = new { },
                    HasSubgroups = subGrpFields.Any(),
                    ItemCount = s.Data.Count(),
                    Items = grpSubItems,
                    Key = s.Key,
                    Member = grpField,
                    SubgroupCount = 0,
                    Subgroups = subGrpFields.Any() ? grpSubItems : new object[] { }
                };
                return item;
            }));
        }

        #endregion
    }
}
