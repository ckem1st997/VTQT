using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Master.Models
{
    public class ReportValueModel : BaseEntityModel, IComparable<ReportValueModel>
    {
        [XBaseResourceDisplayName("Common.Fields.Date")]
        public DateTime Moment { get; set; }

        public string WareHouseId { get; set; }

        public string WareHouseItemId { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Code")]
        public string WareHouseItemCode { get; set; }

        [XBaseResourceDisplayName("WareHouse.WareHouseItems.Fields.Name")]
        public string WareHouseItemName { get; set; }

        [XBaseResourceDisplayName("WareHouse.Units.Fields.UnitName")]
        public string UnitName { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Beginning")]
        public int Beginning { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Inward")]
        public int Import { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Outward")]
        public int Export { get; set; }

        [XBaseResourceDisplayName("WareHouse.Report.Fields.Balance")]
        public int Balance { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Generic")]
        public string Category { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Purpose")]
        public string Purpose { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Proposer")]
        public string User { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Description")]
        public string Description { get; set; }

        [XBaseResourceDisplayName("Common.Fields.DepartmentName")]
        public string DepartmentName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ProjectName")]
        public string ProjectName { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Note")]
        public string NoteRender { get; set; }

        public string DepartmentId { get; set; }

        public string ProjectId { get; set; }

        public string UserId { get; set; }

        public IList<NoteReportModel> Note { get; set; }

        public ReportValueModel()
        {
            Note = new List<NoteReportModel>();
        }

        public int CompareTo([AllowNull] ReportValueModel other)
        {
            if (this.Moment > other.Moment)
            {
                return 1;
            }
            else if (this.Moment == other.Moment)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
