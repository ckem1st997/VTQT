//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Mapping;

using VTQT.Core.Domain.Apps;
using VTQT.Core.Domain.Localization;

namespace VTQT.Core.Domain.Asset
{
	/// <summary>
	/// Database       : AssetManagement
	/// Data Source    : 192.168.100.43
	/// Server Version : 8.0.27-0ubuntu0.20.04.1
	/// </summary>
	public partial class AssetDataConnection : LinqToDB.Data.DataConnection
	{
		/// <summary>
		/// Danh sách tài sản
		/// </summary>
		public ITable<Asset>             Assets             { get { return this.GetTable<Asset>(); } }
		/// <summary>
		/// Loại tài sản
		/// </summary>
		public ITable<AssetCategory>     AssetCategories    { get { return this.GetTable<AssetCategory>(); } }
		/// <summary>
		/// Ghi giảm/thu hồi tài sản
		/// </summary>
		public ITable<AssetDecreased>    AssetDecreaseds    { get { return this.GetTable<AssetDecreased>(); } }
		/// <summary>
		/// Kiểm kê tài sản
		/// </summary>
		public ITable<Audit>             Audits             { get { return this.GetTable<Audit>(); } }
		/// <summary>
		/// Hội đồng kiểm kê
		/// </summary>
		public ITable<AuditCouncil>      AuditCouncils      { get { return this.GetTable<AuditCouncil>(); } }
		/// <summary>
		/// Kết quả kiểm kê
		/// </summary>
		public ITable<AuditDetail>       AuditDetails       { get { return this.GetTable<AuditDetail>(); } }
		/// <summary>
		/// Lý do thu hồi
		/// </summary>
		public ITable<DecreaseReason>    DecreaseReasons    { get { return this.GetTable<DecreaseReason>(); } }
		/// <summary>
		/// Lịch sử
		/// </summary>
		public ITable<History>           Histories          { get { return this.GetTable<History>(); } }
		/// <summary>
		/// Bảo dưỡng/ sửa chữa
		/// </summary>
		public ITable<Maintenance>       Maintenances       { get { return this.GetTable<Maintenance>(); } }
		/// <summary>
		/// Chi tiết của lần sủa chữa
		/// </summary>
		public ITable<MaintenanceDetail> MaintenanceDetails { get { return this.GetTable<MaintenanceDetail>(); } }
		public ITable<Station>           Stations           { get { return this.GetTable<Station>(); } }
		/// <summary>
		/// Danh mục các status
		/// </summary>
		public ITable<UsageStatus>       UsageStatus        { get { return this.GetTable<UsageStatus>(); } }

		public AssetDataConnection()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public AssetDataConnection(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public AssetDataConnection(LinqToDbConnectionOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public AssetDataConnection(LinqToDbConnectionOptions<AssetDataConnection> options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	/// <summary>
	/// Danh sách tài sản
	/// </summary>
	[Serializable]
	[Table("Asset")]
	public partial class Asset : BaseEntity, ILocalizedEntity
	{
		/// <summary>
		/// Mã tài sản
		/// </summary>
		[Column, NotNull    ] public string   Code                 { get; set; } // varchar(50)
		/// <summary>
		/// Tên tài sản
		/// </summary>
		[Column, NotNull    ] public string   Name                 { get; set; } // varchar(100)
		/// <summary>
		/// Trạng thái: 1 - Đang sử dụng; 2 - Tạm ngừng sử dụng; 3 - Đã thu hồi; 4 - Đã thanh lý
		/// </summary>
		[Column, NotNull    ] public int      Status               { get; set; } // int
		/// <summary>
		/// Loại tài sản
		/// </summary>
		[Column,    Nullable] public string   CategoryId           { get; set; } // varchar(36)
		/// <summary>
		/// Tham chiếu
		/// </summary>
		[Column,    Nullable] public string   Reference            { get; set; } // json
		/// <summary>
		/// Mã vật tư
		/// </summary>
		[Column,    Nullable] public string   WareHouseItemCode    { get; set; } // varchar(50)
		/// <summary>
		/// Tên vật tư
		/// </summary>
		[Column,    Nullable] public string   WareHouseItemName    { get; set; } // varchar(100)
		/// <summary>
		/// Số lượng gi tăng
		/// </summary>
		[Column, NotNull    ] public int      OriginQuantity       { get; set; } // int
		/// <summary>
		/// Số lượng đã thu hồi
		/// </summary>
		[Column, NotNull    ] public int      RecallQuantity       { get; set; } // int
		/// <summary>
		/// Số lượng đã hỏng
		/// </summary>
		[Column, NotNull    ] public int      BrokenQuantity       { get; set; } // int
		/// <summary>
		/// Số lượng đã thanh lý
		/// </summary>
		[Column, NotNull    ] public int      SoldQuantity         { get; set; } // int
		/// <summary>
		/// Tình trạng ban đầu
		/// </summary>
		[Column,    Nullable] public string   OriginUsageStatus    { get; set; } // varchar(36)
		/// <summary>
		/// Giá trị ban đầu
		/// </summary>
		[Column, NotNull    ] public decimal  OriginValue          { get; set; } // decimal(15,2)
		/// <summary>
		/// Tình trạng hiện tại
		/// </summary>
		[Column,    Nullable] public string   CurrentUsageStatus   { get; set; } // varchar(36)
		/// <summary>
		/// Giá trị hiện tại
		/// </summary>
		[Column, NotNull    ] public decimal  CurrentValue         { get; set; } // decimal(15,2)
		/// <summary>
		/// Thời gian khấu hao
		/// </summary>
		[Column, NotNull    ] public int      DepreciationDuration { get; set; } // int
		/// <summary>
		/// Khấu hao theo: 1 - Ngày; 2 - Tháng; 3 - Năm
		/// </summary>
		[Column, NotNull    ] public int      DepreciationUnit     { get; set; } // int
		/// <summary>
		/// Thời gian bảo hành
		/// </summary>
		[Column, NotNull    ] public int      WarrantyDuration     { get; set; } // int
		/// <summary>
		/// Bảo hành theo: 1 - Ngày; 2 - Tháng; 3 - Năm
		/// </summary>
		[Column, NotNull    ] public int      WarrantyUnit         { get; set; } // int
		/// <summary>
		/// Điều kiện bảo hành
		/// </summary>
		[Column,    Nullable] public string   WarrantyCondition    { get; set; } // varchar(255)
		/// <summary>
		/// Nhà cung cấp
		/// </summary>
		[Column,    Nullable] public string   VendorName           { get; set; } // varchar(255)
		/// <summary>
		/// Nước sản xuất
		/// </summary>
		[Column,    Nullable] public string   Country              { get; set; } // varchar(255)
		/// <summary>
		/// Năm sản xuất
		/// </summary>
		[Column, NotNull    ] public int      ManufactureYear      { get; set; } // int
		/// <summary>
		/// Mô tả đặc tính
		/// </summary>
		[Column,    Nullable] public string   Description          { get; set; } // varchar(500)
		/// <summary>
		/// Loại tài sản: 1 - Tài sản hành chính; 2 - Tài sản hạ tầng; 3 - Tài sản dự án
		/// </summary>
		[Column, NotNull    ] public int      AssetType            { get; set; } // int
		/// <summary>
		/// Phòng ban
		/// </summary>
		[Column,    Nullable] public string   OrganizationUnitId   { get; set; } // varchar(36)
		/// <summary>
		/// Tên phòng ban
		/// </summary>
		[Column,    Nullable] public string   OrganizationUnitName { get; set; } // varchar(100)
		/// <summary>
		/// Nhân viên
		/// </summary>
		[Column,    Nullable] public string   EmployeeId           { get; set; } // varchar(36)
		/// <summary>
		/// Tên nhân viên
		/// </summary>
		[Column,    Nullable] public string   EmployeeName         { get; set; } // varchar(100)
		/// <summary>
		/// Trạm
		/// </summary>
		[Column,    Nullable] public string   StationCode          { get; set; } // varchar(50)
		/// <summary>
		/// Tên trạm
		/// </summary>
		[Column,    Nullable] public string   StationName          { get; set; } // varchar(100)
		/// <summary>
		/// Dự án
		/// </summary>
		[Column,    Nullable] public string   ProjectCode          { get; set; } // varchar(50)
		/// <summary>
		/// Tên dự án
		/// </summary>
		[Column,    Nullable] public string   ProjectName          { get; set; } // varchar(100)
		/// <summary>
		/// Khách hàng
		/// </summary>
		[Column,    Nullable] public string   CustomerCode         { get; set; } // varchar(50)
		/// <summary>
		/// Tên khách hàng
		/// </summary>
		[Column,    Nullable] public string   CustomerName         { get; set; } // varchar(100)
		/// <summary>
		/// Ngày tạo
		/// </summary>
		[Column, NotNull    ] public DateTime CreatedDate          { get; set; } // datetime
		/// <summary>
		/// Người tạo
		/// </summary>
		[Column,    Nullable] public string   CreatedBy            { get; set; } // varchar(100)
		/// <summary>
		/// Ngày sửa
		/// </summary>
		[Column, NotNull    ] public DateTime ModifiedDate         { get; set; } // timestamp
		/// <summary>
		/// Người sửa
		/// </summary>
		[Column,    Nullable] public string   ModifiedBy           { get; set; } // varchar(100)

		#region Associations

		/// <summary>
		/// FK_Asset_CategoryId
		/// </summary>
		[Association(ThisKey="CategoryId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Asset_CategoryId", BackReferenceName="FK_Asset_CategoryId_BackReferences")]
		public AssetCategory Asset_CategoryId { get; set; }

		/// <summary>
		/// FK_AuditDetails_PK_Asset_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ItemId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AuditDetail> AuditDetails { get; set; }

		/// <summary>
		/// FK_History_AssetId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AssetId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<History> FK_History_AssetId_BackReferences { get; set; }

		/// <summary>
		/// FK_MaintenamceDetail_AssetId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AssetId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<MaintenanceDetail> FK_MaintenamceDetail_AssetId_BackReferences { get; set; }

		#endregion
	}

	/// <summary>
	/// Loại tài sản
	/// </summary>
	[Serializable]
	[Table("AssetCategory")]
	public partial class AssetCategory : BaseEntity, ILocalizedEntity
	{
		/// <summary>
		/// Mã
		/// </summary>
		[Column, NotNull    ] public string Code                 { get; set; } // varchar(20)
		/// <summary>
		/// Tên
		/// </summary>
		[Column, NotNull    ] public string Name                 { get; set; } // varchar(100)
		/// <summary>
		/// Loại cha
		/// </summary>
		[Column,    Nullable] public string ParentId             { get; set; } // varchar(36)
		[Column,    Nullable] public string Path                 { get; set; } // varchar(255)
		/// <summary>
		/// Mô tả
		/// </summary>
		[Column, NotNull    ] public string Description          { get; set; } // varchar(255)
		/// <summary>
		/// Thời gian khấu hao
		/// </summary>
		[Column,    Nullable] public int?   DepreciationDuration { get; set; } // int
		/// <summary>
		/// Đơn vị của thời gian khấu hao (1: Ngày, 2: Tháng, 3: Năm)
		/// </summary>
		[Column,    Nullable] public int?   DepreciationUnit     { get; set; } // int
		/// <summary>
		/// Thời gian bảo hành
		/// </summary>
		[Column,    Nullable] public int?   WarrantyDuration     { get; set; } // int
		/// <summary>
		/// Đơn vị của thời gian bảo hành (1: Ngày, 2: Tháng, 3: Năm)
		/// </summary>
		[Column,    Nullable] public int?   WarrantyUnit         { get; set; } // int
		[Column, NotNull    ] public bool   Inactive             { get; set; } // bit(1)

		#region Associations

		/// <summary>
		/// FK_AssetCategory_ParentId
		/// </summary>
		[Association(ThisKey="ParentId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AssetCategory_ParentId", BackReferenceName="FK_AssetCategory_ParentId_BackReferences")]
		public AssetCategory AssetCategory_ParentId { get; set; }

		/// <summary>
		/// FK_Asset_CategoryId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="CategoryId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Asset> FK_Asset_CategoryId_BackReferences { get; set; }

		/// <summary>
		/// FK_AssetCategory_ParentId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ParentId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AssetCategory> FK_AssetCategory_ParentId_BackReferences { get; set; }

		#endregion
	}

	/// <summary>
	/// Ghi giảm/thu hồi tài sản
	/// </summary>
	[Serializable]
	[Table("AssetDecreased")]
	public partial class AssetDecreased : BaseEntity
	{
		[Column, NotNull    ] public string   AssetId        { get; set; } // varchar(36)
		/// <summary>
		/// Lý do ghi giảm
		/// </summary>
		[Column, NotNull    ] public string   DecreaseReason { get; set; } // varchar(36)
		[Column, NotNull    ] public int      Quantity       { get; set; } // int
		[Column, NotNull    ] public DateTime DecreaseDate   { get; set; } // datetime
		[Column,    Nullable] public string   EmployeeId     { get; set; } // varchar(36)
		[Column,    Nullable] public string   EmployeeName   { get; set; } // varchar(100)
		[Column,    Nullable] public string   WareHouseCode  { get; set; } // varchar(50)
		[Column,    Nullable] public string   WareHouseName  { get; set; } // varchar(255)
		[Column,    Nullable] public string   Notes          { get; set; } // varchar(500)
		[Column,    Nullable] public string   CreatedBy      { get; set; } // varchar(100)
		[Column, NotNull    ] public DateTime CreatedDate    { get; set; } // datetime
		[Column,    Nullable] public string   ModifiedBy     { get; set; } // varchar(100)
		[Column, NotNull    ] public DateTime ModifiedDate   { get; set; } // timestamp
	}

	/// <summary>
	/// Kiểm kê tài sản
	/// </summary>
	[Serializable]
	[Table("Audit")]
	public partial class Audit : BaseEntity
	{
		/// <summary>
		/// Số phiếu
		/// </summary>
		[Column, NotNull    ] public string   VoucherCode   { get; set; } // varchar(20)
		/// <summary>
		/// Ngày phiếu
		/// </summary>
		[Column, NotNull    ] public DateTime VoucherDate   { get; set; } // datetime
		/// <summary>
		/// Loại tài sản
		/// </summary>
		[Column, NotNull    ] public int      AssetType     { get; set; } // int
		/// <summary>
		/// Nơi thực hiện kiểm kê: Nếu là tài sản hành chính thì là phòng ban, nếu tài sản là dự án thì là mã dự án, nếu tài sản hạ tâng thì là mã trạm ...
		/// </summary>
		[Column, NotNull    ] public string   AuditLocation { get; set; } // varchar(36)
		/// <summary>
		/// Tên đợt kiểm kê
		/// </summary>
		[Column,    Nullable] public string   Description   { get; set; } // varchar(255)
		/// <summary>
		/// Ngày tạo
		/// </summary>
		[Column, NotNull    ] public DateTime CreatedDate   { get; set; } // datetime
		/// <summary>
		/// Người tạo
		/// </summary>
		[Column,    Nullable] public string   CreatedBy     { get; set; } // varchar(100)
		/// <summary>
		/// Ngày sửa
		/// </summary>
		[Column, NotNull    ] public DateTime ModifiedDate  { get; set; } // timestamp
		[Column,    Nullable] public string   ModifiedBy    { get; set; } // varchar(100)

		#region Associations

		/// <summary>
		/// FK_AuditCouncils_PK_Audit_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AuditId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AuditCouncil> AuditCouncils { get; set; }

		/// <summary>
		/// FK_AuditDetails_PK_Audit_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AuditId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AuditDetail> AuditDetails { get; set; }

		#endregion
	}

	/// <summary>
	/// Hội đồng kiểm kê
	/// </summary>
	[Serializable]
	[Table("AuditCouncil")]
	public partial class AuditCouncil : BaseEntity
	{
		/// <summary>
		/// fk kỳ kiểm kê
		/// </summary>
		[Column, NotNull    ] public string AuditId      { get; set; } // varchar(36)
		/// <summary>
		/// fk nhân viên
		/// </summary>
		[Column,    Nullable] public string EmployeeId   { get; set; } // varchar(36)
		/// <summary>
		/// Tên nhân viên
		/// </summary>
		[Column,    Nullable] public string EmployeeName { get; set; } // varchar(100)
		/// <summary>
		/// vai trò trong đoàn kiểm kê
		/// </summary>
		[Column,    Nullable] public string Role         { get; set; } // varchar(100)

		#region Associations

		/// <summary>
		/// FK_AuditCouncils_PK_Audit
		/// </summary>
		[Association(ThisKey="AuditId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AuditCouncils_PK_Audit", BackReferenceName="AuditCouncils")]
		public Audit Audit { get; set; }

		#endregion
	}

	/// <summary>
	/// Kết quả kiểm kê
	/// </summary>
	[Serializable]
	[Table("AuditDetail")]
	public partial class AuditDetail : BaseEntity
	{
		/// <summary>
		/// FK Audit
		/// </summary>
		[Column, NotNull    ] public string AuditId       { get; set; } // varchar(36)
		/// <summary>
		/// fk Vật tài sản
		/// </summary>
		[Column,    Nullable] public string ItemId        { get; set; } // varchar(36)
		/// <summary>
		/// Số lượng sổ sách
		/// </summary>
		[Column, NotNull    ] public int    Quantity      { get; set; } // int
		/// <summary>
		/// Số lượng thực tế
		/// </summary>
		[Column, NotNull    ] public int    AuditQuantity { get; set; } // int
		/// <summary>
		/// Kết luận
		/// </summary>
		[Column,    Nullable] public string Conclude      { get; set; } // varchar(255)

		#region Associations

		/// <summary>
		/// FK_AuditDetails_PK_Asset
		/// </summary>
		[Association(ThisKey="ItemId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AuditDetails_PK_Asset", BackReferenceName="AuditDetails")]
		public Asset Asset { get; set; }

		/// <summary>
		/// FK_AuditDetails_PK_Audit
		/// </summary>
		[Association(ThisKey="AuditId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AuditDetails_PK_Audit", BackReferenceName="AuditDetails")]
		public Audit Audit { get; set; }

		#endregion
	}

	/// <summary>
	/// Lý do thu hồi
	/// </summary>
	[Serializable]
	[Table("DecreaseReason")]
	public partial class DecreaseReason : BaseEntity
	{
		[Column,    Nullable] public string Code { get; set; } // varchar(20)
		[Column,    Nullable] public string Name { get; set; } // varchar(100)
	}

	/// <summary>
	/// Lịch sử
	/// </summary>
	[Serializable]
	[Table("History")]
	public partial class History : BaseEntity
	{
		/// <summary>
		/// FK tài sản
		/// </summary>
		[Column,    Nullable] public string   AssetId   { get; set; } // varchar(36)
		/// <summary>
		/// Hành động: Thêm, Sửa, Sủa chữa, Thu hồi, Bảo dưỡng, Thanh Lý ...
		/// </summary>
		[Column,    Nullable] public string   Action    { get; set; } // varchar(50)
		/// <summary>
		/// Nội dung tóm tắt
		/// </summary>
		[Column,    Nullable] public string   Content   { get; set; } // varchar(255)
		/// <summary>
		/// Nguwoif thực hiện
		/// </summary>
		[Column, NotNull    ] public string   User      { get; set; } // varchar(100)
		/// <summary>
		/// Thời gian
		/// </summary>
		[Column, NotNull    ] public DateTime TimeStamp { get; set; } // timestamp

		#region Associations

		/// <summary>
		/// FK_History_AssetId
		/// </summary>
		[Association(ThisKey="AssetId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_History_AssetId", BackReferenceName="FK_History_AssetId_BackReferences")]
		public Asset History_AssetId { get; set; }

		#endregion
	}

	/// <summary>
	/// Bảo dưỡng/ sửa chữa
	/// </summary>
	[Serializable]
	[Table("Maintenance")]
	public partial class Maintenance : BaseEntity
	{
		[Column,    Nullable] public string   EmployeeId       { get; set; } // varchar(36)
		/// <summary>
		/// Người thực hiện
		/// </summary>
		[Column,    Nullable] public string   EmployeeName     { get; set; } // varchar(100)
		/// <summary>
		/// Sủa chữa, bảo dưỡng định kỳ
		/// </summary>
		[Column,    Nullable] public string   Action           { get; set; } // varchar(50)
		/// <summary>
		/// Nội dung
		/// </summary>
		[Column,    Nullable] public string   Content          { get; set; } // varchar(255)
		/// <summary>
		/// Chi phí
		/// </summary>
		[Column, NotNull    ] public decimal  Amount           { get; set; } // decimal(15,2)
		[Column, NotNull    ] public DateTime MaintenancedDate { get; set; } // datetime

		#region Associations

		/// <summary>
		/// FK_MaintenamceDetail_MaintenanceId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="MaintenanceId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<MaintenanceDetail> FK_MaintenamceDetail_MaintenanceId_BackReferences { get; set; }

		#endregion
	}

	/// <summary>
	/// Chi tiết của lần sủa chữa
	/// </summary>
	[Serializable]
	[Table("MaintenanceDetail")]
	public partial class MaintenanceDetail : BaseEntity
	{
		[Column,    Nullable] public string MaintenanceId     { get; set; } // varchar(36)
		[Column,    Nullable] public string AssetId           { get; set; } // varchar(36)
		[Column,    Nullable] public string ReasonDescription { get; set; } // varchar(255)

		#region Associations

		/// <summary>
		/// FK_MaintenamceDetail_AssetId
		/// </summary>
		[Association(ThisKey="AssetId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_MaintenamceDetail_AssetId", BackReferenceName="FK_MaintenamceDetail_AssetId_BackReferences")]
		public Asset MaintenamceDetail_AssetId { get; set; }

		/// <summary>
		/// FK_MaintenamceDetail_MaintenanceId
		/// </summary>
		[Association(ThisKey="MaintenanceId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_MaintenamceDetail_MaintenanceId", BackReferenceName="FK_MaintenamceDetail_MaintenanceId_BackReferences")]
		public Maintenance MaintenamceDetail_MaintenanceId { get; set; }

		#endregion
	}

	[Serializable]
	[Table("Station")]
	public partial class Station : BaseEntity
	{
		/// <summary>
		/// Khu vực
		/// </summary>
		[Column,    Nullable] public string  Area       { get; set; } // varchar(50)
		/// <summary>
		/// Tên tỉnh
		/// </summary>
		[Column,    Nullable] public string  Province   { get; set; } // varchar(255)
		/// <summary>
		/// Tên trạm
		/// </summary>
		[Column,    Nullable] public string  Name       { get; set; } // varchar(45)
		/// <summary>
		/// Mã trạm
		/// </summary>
		[Column,    Nullable] public string  Code       { get; set; } // varchar(255)
		/// <summary>
		/// Mã trạm VNM
		/// </summary>
		[Column,    Nullable] public string  CodeVnm    { get; set; } // varchar(255)
		/// <summary>
		/// Phân cấp
		/// </summary>
		[Column,    Nullable] public int?    Level      { get; set; } // int
		/// <summary>
		/// Địa chỉ
		/// </summary>
		[Column,    Nullable] public string  Address    { get; set; } // varchar(255)
		/// <summary>
		/// Kinh độ
		/// </summary>
		[Column,    Nullable] public double? Longitude  { get; set; } // double
		/// <summary>
		/// Vĩ độ
		/// </summary>
		[Column,    Nullable] public double? Latitude   { get; set; } // double
		/// <summary>
		/// Ghi chú
		/// </summary>
		[Column,    Nullable] public string  Note       { get; set; } // varchar(255)
		/// <summary>
		/// Loại trạm
		/// </summary>
		[Column,    Nullable] public int?    CategoryId { get; set; } // int
	}

	/// <summary>
	/// Danh mục các status
	/// </summary>
	[Serializable]
	[Table("UsageStatus")]
	public partial class UsageStatus : BaseEntity
	{
		/// <summary>
		/// Mã trạng thái
		/// </summary>
		[Column, NotNull    ] public string Status      { get; set; } // varchar(50)
		[Column,    Nullable] public string Description { get; set; } // varchar(255)
		[Column, NotNull    ] public bool   Inactive    { get; set; } // bit(1)
	}

	public static partial class TableExtensions
	{
		public static Asset Find(this ITable<Asset> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AssetCategory Find(this ITable<AssetCategory> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AssetDecreased Find(this ITable<AssetDecreased> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Audit Find(this ITable<Audit> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AuditCouncil Find(this ITable<AuditCouncil> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AuditDetail Find(this ITable<AuditDetail> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static DecreaseReason Find(this ITable<DecreaseReason> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static History Find(this ITable<History> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Maintenance Find(this ITable<Maintenance> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static MaintenanceDetail Find(this ITable<MaintenanceDetail> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Station Find(this ITable<Station> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UsageStatus Find(this ITable<UsageStatus> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}

#pragma warning restore 1591
