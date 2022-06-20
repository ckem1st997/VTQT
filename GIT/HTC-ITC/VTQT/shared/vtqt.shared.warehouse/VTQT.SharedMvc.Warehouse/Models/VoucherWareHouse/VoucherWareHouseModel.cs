using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
    public class VoucherWareHouseModel : BaseEntityModel, IComparable<VoucherWareHouseModel>
    {
        [XBaseResourceDisplayName("WareHouse.VoucherWareHouses.Fields.VoucherType")]
        public string VoucherType { get; set; }

        [XBaseResourceDisplayName("Common.Fields.VoucherCode")]
        public string VoucherCode { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Voucher")]
        public string Voucher { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Date")]
        public DateTime VoucherDate { get; set; }

        [XBaseResourceDisplayName("Common.Fields.CreatedBy")]
        public string CreatedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ModifiedBy")]
        public string ModifiedBy { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Deliver")]
        public string Deliver { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Receiver")]
        public string Receiver { get; set; }

        [XBaseResourceDisplayName("Common.Fields.ReasonDescription")]
        public string ReasonDescription { get; set; }
        
        [XBaseResourceDisplayName("Common.Fields.Reason")]
        public int Reason { get; set; }

        [XBaseResourceDisplayName("Common.Fields.Date")]
        public string StrVoucherDate { get; set; }
        
        [XBaseResourceDisplayName("Common.Fields.SelectedInwardReason")]
        public string SelectedInwardReason { get; set; }
                
        [XBaseResourceDisplayName("Common.Fields.SelectedOutwardReason")]
        public string SelectedOutwardReason { get; set; }

        [XBaseResourceDisplayName("Common.Fields.User")]
        public UserModel UserModel { get; set; }

        [XBaseResourceDisplayName("Common.VoucherWareHouse.Fields.Description")]
        public string Description { get; set; }

        public IList<SelectListItem> AvailableCreatedBy { get; set; }

        public VoucherWareHouseModel()
        {
            AvailableCreatedBy = new List<SelectListItem>();
        }

        public int CompareTo([AllowNull] VoucherWareHouseModel other)
        {
            if (this.VoucherDate > other.VoucherDate)
            {
                return -1;
            }
            else if (this.VoucherDate == other.VoucherDate)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
