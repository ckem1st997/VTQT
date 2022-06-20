using System;
using System.Collections.Generic;
using System.Text;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Warehouse.Models
{
	public partial class AccObjectModel : BaseEntityModel
	{
		/// <summary>
		/// Mã đối tượng
		/// </summary>
		public string Code { get; set; } // varchar(20)
		/// <summary>
		/// Tên đối tượng
		/// </summary>
		public string Name { get; set; } // varchar(255)
		/// <summary>
		/// Địa chỉ
		/// </summary>
		public string Address { get; set; } // varchar(255)
		/// <summary>
		/// Số điện thoại
		/// </summary>
		public string Phone { get; set; } // varchar(20)
		/// <summary>
		/// Ngừng theo dõi
		/// </summary>
		public bool Inactive { get; set; } // bit(1)

		#region Associations

		/// <summary>
		/// FK_WareHouseInwards_PK_AccObject_BackReference
		/// </summary>
		public IEnumerable<InwardModel> WareHouseInwards { get; set; }

		/// <summary>
		/// FK_WareHouseInwardsDetails_PK_AccObject_BackReference
		/// </summary>
		public IEnumerable<InwardDetailModel> WareHouseInwardsDetails { get; set; }

		/// <summary>
		/// FK_WareHouseOutwardDetail_PK_AccObject_BackReference
		/// </summary>
		public IEnumerable<OutwardDetailModel> WareHouseOutwardDetails { get; set; }

		/// <summary>
		/// FK_WareHouseOutward_PK_AccObject_BackReference
		/// </summary>
		public IEnumerable<OutwardModel> WareHouseOutwards { get; set; }

		#endregion
	}
}