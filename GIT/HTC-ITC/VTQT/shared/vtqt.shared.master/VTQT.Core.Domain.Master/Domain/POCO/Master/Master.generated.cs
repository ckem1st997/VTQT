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

namespace VTQT.Core.Domain.Master
{
	/// <summary>
	/// Database       : MasterData
	/// Data Source    : 192.168.100.43
	/// Server Version : 8.0.26-0ubuntu0.20.04.2
	/// </summary>
	public partial class MasterDataConnection : LinqToDB.Data.DataConnection
	{
		/// <summary>
		/// Danh sách Ứng dụng
		/// </summary>
		public ITable<App>                    Apps                    { get { return this.GetTable<App>(); } }
		public ITable<AppAction>              AppActions              { get { return this.GetTable<AppAction>(); } }
		public ITable<AppActionRole>          AppActionRoles          { get { return this.GetTable<AppActionRole>(); } }
		public ITable<AppActionUserExclusion> AppActionUserExclusions { get { return this.GetTable<AppActionUserExclusion>(); } }
		public ITable<AppActionUserInclusion> AppActionUserInclusions { get { return this.GetTable<AppActionUserInclusion>(); } }
		/// <summary>
		/// App Mapping
		/// </summary>
		public ITable<AppMapping>             AppMappings             { get { return this.GetTable<AppMapping>(); } }
		/// <summary>
		/// Danh sách Đơn vị tiền tệ
		/// </summary>
		public ITable<Currency>               Currencies              { get { return this.GetTable<Currency>(); } }
		/// <summary>
		/// Danh sách Ngôn ngữ hệ thống
		/// </summary>
		public ITable<Language>               Languages               { get { return this.GetTable<Language>(); } }
		/// <summary>
		/// Tài nguyên đa ngôn ngữ
		/// </summary>
		public ITable<LocaleStringResource>   LocaleStringResources   { get { return this.GetTable<LocaleStringResource>(); } }
		/// <summary>
		/// Trường dữ liệu đa ngôn ngữ
		/// </summary>
		public ITable<LocalizedProperty>      LocalizedProperties     { get { return this.GetTable<LocalizedProperty>(); } }
		/// <summary>
		/// Danh sách báo cáo
		/// </summary>
		public ITable<Report>                 Reports                 { get { return this.GetTable<Report>(); } }
		/// <summary>
		/// Danh sách Vai trò người dùng
		/// </summary>
		public ITable<Role>                   Roles                   { get { return this.GetTable<Role>(); } }
		/// <summary>
		/// Thiết lập hệ thống
		/// </summary>
		public ITable<Setting>                Settings                { get { return this.GetTable<Setting>(); } }
		/// <summary>
		/// Danh sách Url tùy chỉnh, SEO
		/// </summary>
		public ITable<UrlRecord>              UrlRecords              { get { return this.GetTable<UrlRecord>(); } }
		public ITable<UserRole>               UserRoles               { get { return this.GetTable<UserRole>(); } }

		public MasterDataConnection()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public MasterDataConnection(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public MasterDataConnection(LinqToDbConnectionOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public MasterDataConnection(LinqToDbConnectionOptions<MasterDataConnection> options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	/// <summary>
	/// Danh sách Ứng dụng
	/// </summary>
	[Serializable]
	[Table("App")]
	public partial class App : BaseEntity, ILocalizedEntity
	{
		/// <summary>
		/// Loại ứng dụng
		/// </summary>
		[Column, NotNull    ] public string AppType           { get; set; } // varchar(255)
		/// <summary>
		/// Tên ứng dụng (đa ngôn ngữ)
		/// </summary>
		[Column, NotNull    ] public string Name              { get; set; } // varchar(255)
		/// <summary>
		/// Tên viết tắt
		/// </summary>
		[Column, NotNull    ] public string ShortName         { get; set; } // varchar(255)
		/// <summary>
		/// Mô tả (đa ngôn ngữ)
		/// </summary>
		[Column,    Nullable] public string Description       { get; set; } // longtext
		/// <summary>
		/// Icon
		/// </summary>
		[Column,    Nullable] public string Icon              { get; set; } // varchar(1000)
		/// <summary>
		/// Màu nền
		/// </summary>
		[Column,    Nullable] public string BackgroundColor   { get; set; } // varchar(1000)
		/// <summary>
		/// Đường dẫn
		/// </summary>
		[Column, NotNull    ] public string Url               { get; set; } // varchar(400)
		/// <summary>
		/// Hosts
		/// </summary>
		[Column,    Nullable] public string Hosts             { get; set; } // varchar(1000)
		/// <summary>
		/// Kích hoạt SSL
		/// </summary>
		[Column, NotNull    ] public bool   SslEnabled        { get; set; } // tinyint(1)
		/// <summary>
		/// Cdn Url
		/// </summary>
		[Column,    Nullable] public string CdnUrl            { get; set; } // varchar(400)
		/// <summary>
		/// Ngôn ngữ mặc định (Ref: Language)
		/// </summary>
		[Column, NotNull    ] public string DefaultLanguageId { get; set; } // varchar(36)
		/// <summary>
		/// Hiển thị lên menu
		/// </summary>
		[Column, NotNull    ] public bool   ShowOnMenu        { get; set; } // tinyint(1)
		/// <summary>
		/// Thứ tự hiển thị
		/// </summary>
		[Column, NotNull    ] public int    DisplayOrder      { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_AppActions_PK_App_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AppId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppAction> AppActions { get; set; }

		/// <summary>
		/// FK_AppMappings_PK_App_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AppId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppMapping> AppMappings { get; set; }

		/// <summary>
		/// FK_Report_AppId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AppId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Report> FK_Report_AppId_BackReferences { get; set; }

		#endregion
	}

	[Serializable]
	[Table("AppAction")]
	public partial class AppAction : BaseEntity, ILocalizedEntity
	{
		[Column, NotNull    ] public string AppId        { get; set; } // varchar(36)
		[Column,    Nullable] public string ParentId     { get; set; } // varchar(36)
		[Column, NotNull    ] public string Name         { get; set; } // varchar(255)
		[Column,    Nullable] public string Description  { get; set; } // longtext
		[Column,    Nullable] public string Controller   { get; set; } // varchar(255)
		[Column,    Nullable] public string Action       { get; set; } // varchar(255)
		[Column,    Nullable] public string Icon         { get; set; } // varchar(1000)
		[Column, NotNull    ] public bool   ShowOnMenu   { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool   Active       { get; set; } // tinyint(1)
		[Column, NotNull    ] public int    DisplayOrder { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_AppActions_PK_App
		/// </summary>
		[Association(ThisKey="AppId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppActions_PK_App", BackReferenceName="AppActions")]
		public App App { get; set; }

		/// <summary>
		/// FK_AppActionChildren_PK_Parent_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ParentId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppAction> AppActionChildren { get; set; }

		/// <summary>
		/// FK_AppActionRoles_PK_AppAction_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AppActionId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppActionRole> AppActionRoles { get; set; }

		/// <summary>
		/// FK_AppActionUserExclusions_PK_AppAction_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ExcludeAppActionId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppActionUserExclusion> AppActionUserExclusions { get; set; }

		/// <summary>
		/// FK_AppActionUserInclusions_PK_AppAction_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="IncludeAppActionId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppActionUserInclusion> AppActionUserInclusions { get; set; }

		/// <summary>
		/// FK_AppActionChildren_PK_Parent
		/// </summary>
		[Association(ThisKey="ParentId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppActionChildren_PK_Parent", BackReferenceName="AppActionChildren")]
		public AppAction Parent { get; set; }

		#endregion
	}

	[Serializable]
	[Table("AppActionRole")]
	public partial class AppActionRole : BaseEntity
	{
		[Column, NotNull] public string RoleId      { get; set; } // varchar(36)
		[Column, NotNull] public string AppActionId { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_AppActionRoles_PK_AppAction
		/// </summary>
		[Association(ThisKey="AppActionId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppActionRoles_PK_AppAction", BackReferenceName="AppActionRoles")]
		public AppAction AppAction { get; set; }

		/// <summary>
		/// FK_AppActionRoles_PK_Role
		/// </summary>
		[Association(ThisKey="RoleId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppActionRoles_PK_Role", BackReferenceName="AppActionRoles")]
		public Role Role { get; set; }

		#endregion
	}

	[Serializable]
	[Table("AppActionUserExclusion")]
	public partial class AppActionUserExclusion : BaseEntity
	{
		[Column, NotNull] public string ExcludeUserId      { get; set; } // varchar(36)
		[Column, NotNull] public string ExcludeAppActionId { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_AppActionUserExclusions_PK_AppAction
		/// </summary>
		[Association(ThisKey="ExcludeAppActionId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppActionUserExclusions_PK_AppAction", BackReferenceName="AppActionUserExclusions")]
		public AppAction AppAction { get; set; }

		#endregion
	}

	[Serializable]
	[Table("AppActionUserInclusion")]
	public partial class AppActionUserInclusion : BaseEntity
	{
		[Column, NotNull] public string IncludeUserId      { get; set; } // varchar(36)
		[Column, NotNull] public string IncludeAppActionId { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_AppActionUserInclusions_PK_AppAction
		/// </summary>
		[Association(ThisKey="IncludeAppActionId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppActionUserInclusions_PK_AppAction", BackReferenceName="AppActionUserInclusions")]
		public AppAction AppAction { get; set; }

		#endregion
	}

	/// <summary>
	/// App Mapping
	/// </summary>
	[Serializable]
	[Table("AppMapping")]
	public partial class AppMapping : BaseEntity
	{
		/// <summary>
		/// Tên Entity
		/// </summary>
		[Column, NotNull] public string EntityName { get; set; } // varchar(255)
		/// <summary>
		/// App (FK: App)
		/// </summary>
		[Column, NotNull] public string AppId      { get; set; } // varchar(36)
		/// <summary>
		/// Entity (Ref: BaseEntity)
		/// </summary>
		[Column, NotNull] public string EntityId   { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_AppMappings_PK_App
		/// </summary>
		[Association(ThisKey="AppId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_AppMappings_PK_App", BackReferenceName="AppMappings")]
		public App App { get; set; }

		#endregion
	}

	/// <summary>
	/// Danh sách Đơn vị tiền tệ
	/// </summary>
	[Serializable]
	[Table("Currency")]
	public partial class Currency : BaseEntity, ILocalizedEntity, IAppMappingSupported
	{
		/// <summary>
		/// Tên tiền tệ (đa ngôn ngữ)
		/// </summary>
		[Column, NotNull    ] public string   Name             { get; set; } // varchar(50)
		/// <summary>
		/// Mã tiền tệ
		/// </summary>
		[Column, NotNull    ] public string   CurrencyCode     { get; set; } // varchar(5)
		/// <summary>
		/// Mã ngôn ngữ địa phương (VD: en-US, vi-VN)
		/// </summary>
		[Column,    Nullable] public string   DisplayLocale    { get; set; } // varchar(50)
		/// <summary>
		/// Định dạng tùy chỉnh
		/// </summary>
		[Column,    Nullable] public string   CustomFormatting { get; set; } // varchar(50)
		/// <summary>
		/// Tỷ giá
		/// </summary>
		[Column, NotNull    ] public decimal  Rate             { get; set; } // decimal(18,4)
		/// <summary>
		/// Giới hạn cho các App
		/// </summary>
		[Column, NotNull    ] public bool     LimitedToApps    { get; set; } // tinyint(1)
		/// <summary>
		/// Phát hành
		/// </summary>
		[Column, NotNull    ] public bool     Published        { get; set; } // tinyint(1)
		/// <summary>
		/// Thứ tự hiển thị
		/// </summary>
		[Column, NotNull    ] public int      DisplayOrder     { get; set; } // int
		/// <summary>
		/// Ngày giờ tạo
		/// </summary>
		[Column, NotNull    ] public DateTime CreatedOnUtc     { get; set; } // datetime
		/// <summary>
		/// Ngày giờ sửa
		/// </summary>
		[Column, NotNull    ] public DateTime UpdatedOnUtc     { get; set; } // datetime
	}

	/// <summary>
	/// Danh sách Ngôn ngữ hệ thống
	/// </summary>
	[Serializable]
	[Table("Language")]
	public partial class Language : BaseEntity, IAppMappingSupported
	{
		/// <summary>
		/// Tên ngôn ngữ
		/// </summary>
		[Column, NotNull    ] public string Name              { get; set; } // varchar(100)
		/// <summary>
		/// Mã ngôn ngữ địa phương (VD: en-US, vi-VN)
		/// </summary>
		[Column, NotNull    ] public string LanguageCulture   { get; set; } // varchar(20)
		/// <summary>
		/// Mã ngôn ngữ ISO 2 ký tự (VD: en, vi)
		/// </summary>
		[Column,    Nullable] public string UniqueSeoCode     { get; set; } // varchar(2)
		/// <summary>
		/// Tên file cờ quốc gia
		/// </summary>
		[Column,    Nullable] public string FlagImageFileName { get; set; } // varchar(50)
		/// <summary>
		/// Bố cục phải-qua-trái
		/// </summary>
		[Column, NotNull    ] public bool   Rtl               { get; set; } // tinyint(1)
		/// <summary>
		/// Giới hạn cho các App
		/// </summary>
		[Column, NotNull    ] public bool   LimitedToApps     { get; set; } // tinyint(1)
		/// <summary>
		/// Tiền tệ mặc định (Ref: Currency)
		/// </summary>
		[Column,    Nullable] public string DefaultCurrencyId { get; set; } // varchar(36)
		/// <summary>
		/// Phát hành
		/// </summary>
		[Column, NotNull    ] public bool   Published         { get; set; } // tinyint(1)
		/// <summary>
		/// Thứ tự hiển thị
		/// </summary>
		[Column, NotNull    ] public int    DisplayOrder      { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_LocaleStringResources_PK_Language_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="LanguageId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<LocaleStringResource> LocaleStringResources { get; set; }

		/// <summary>
		/// FK_LocalizedProperties_PK_Language_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="LanguageId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<LocalizedProperty> LocalizedProperties { get; set; }

		#endregion
	}

	/// <summary>
	/// Tài nguyên đa ngôn ngữ
	/// </summary>
	[Serializable]
	[Table("LocaleStringResource")]
	public partial class LocaleStringResource : BaseEntity
	{
		/// <summary>
		/// Tên tài nguyên
		/// </summary>
		[Column, NotNull] public string ResourceName  { get; set; } // varchar(200)
		/// <summary>
		/// Giá trị tài nguyên
		/// </summary>
		[Column, NotNull] public string ResourceValue { get; set; } // longtext
		/// <summary>
		/// Ngôn ngữ (FK: Language)
		/// </summary>
		[Column, NotNull] public string LanguageId    { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_LocaleStringResources_PK_Language
		/// </summary>
		[Association(ThisKey="LanguageId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_LocaleStringResources_PK_Language", BackReferenceName="LocaleStringResources")]
		public Language Language { get; set; }

		#endregion
	}

	/// <summary>
	/// Trường dữ liệu đa ngôn ngữ
	/// </summary>
	[Serializable]
	[Table("LocalizedProperty")]
	public partial class LocalizedProperty : BaseEntity
	{
		/// <summary>
		/// Tên Entity
		/// </summary>
		[Column, NotNull] public string LocaleKeyGroup { get; set; } // varchar(255)
		/// <summary>
		/// Entity Property
		/// </summary>
		[Column, NotNull] public string LocaleKey      { get; set; } // varchar(255)
		/// <summary>
		/// Giá trị Property
		/// </summary>
		[Column, NotNull] public string LocaleValue    { get; set; } // longtext
		/// <summary>
		/// Ngôn ngữ (FK: Language)
		/// </summary>
		[Column, NotNull] public string LanguageId     { get; set; } // varchar(36)
		/// <summary>
		/// Entity (Ref: BaseEntity)
		/// </summary>
		[Column, NotNull] public string EntityId       { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_LocalizedProperties_PK_Language
		/// </summary>
		[Association(ThisKey="LanguageId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_LocalizedProperties_PK_Language", BackReferenceName="LocalizedProperties")]
		public Language Language { get; set; }

		#endregion
	}

	/// <summary>
	/// Danh sách báo cáo
	/// </summary>
	[Serializable]
	[Table("Report")]
	public partial class Report : BaseEntity
	{
		/// <summary>
		/// Tên báo cáo
		/// </summary>
		[Column,    Nullable] public string Name           { get; set; } // varchar(255)
		/// <summary>
		/// Mô tả báo cáo
		/// </summary>
		[Column,    Nullable] public string Description    { get; set; } // varchar(255)
		/// <summary>
		/// Ứng dụng
		/// </summary>
		[Column,    Nullable] public string AppId          { get; set; } // varchar(36)
		/// <summary>
		/// Loại: 1 - báo cáo ; 2 - phiếu in
		/// </summary>
		[Column, NotNull    ] public int    Type           { get; set; } // int
		/// <summary>
		/// Đường dẫn đến Route của report
		/// </summary>
		[Column,    Nullable] public string Route          { get; set; } // varchar(255)
		/// <summary>
		/// Đường dẫn đến file mẫu báo cáo -&gt; dùng để render ra nội dung báo cáo
		/// </summary>
		[Column,    Nullable] public string ReportTemplate { get; set; } // varchar(255)
		/// <summary>
		/// File mẫu excel -&gt; dùng để làm báo cáo theo mẫu excel để xuất ra
		/// </summary>
		[Column,    Nullable] public string ExcelTemplate  { get; set; } // varchar(255)
		/// <summary>
		/// Thứ tự hiển thị
		/// </summary>
		[Column, NotNull    ] public int    Order          { get; set; } // int
		[Column, NotNull    ] public bool   Inactive       { get; set; } // bit(1)

		#region Associations

		/// <summary>
		/// FK_Report_AppId
		/// </summary>
		[Association(ThisKey="AppId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Report_AppId", BackReferenceName="FK_Report_AppId_BackReferences")]
		public App Report_AppId { get; set; }

		#endregion
	}

	/// <summary>
	/// Danh sách Vai trò người dùng
	/// </summary>
	[Serializable]
	[Table("Role")]
	public partial class Role : BaseEntity, ILocalizedEntity
	{
		/// <summary>
		/// Tên vai trò (SystemName)
		/// </summary>
		[Column, NotNull    ] public string Name         { get; set; } // varchar(255)
		/// <summary>
		/// Tên hiển thị (đa ngôn ngữ)
		/// </summary>
		[Column, NotNull    ] public string DisplayName  { get; set; } // varchar(255)
		/// <summary>
		/// Mô tả (đa ngôn ngữ)
		/// </summary>
		[Column,    Nullable] public string Description  { get; set; } // longtext
		/// <summary>
		/// Kích hoạt
		/// </summary>
		[Column, NotNull    ] public bool   Active       { get; set; } // tinyint(1)
		/// <summary>
		/// Thứ tự hiển thị
		/// </summary>
		[Column, NotNull    ] public int    DisplayOrder { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_AppActionRoles_PK_Role_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="RoleId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<AppActionRole> AppActionRoles { get; set; }

		/// <summary>
		/// FK_UserRoles_PK_Role_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="RoleId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<UserRole> UserRoles { get; set; }

		#endregion
	}

	/// <summary>
	/// Thiết lập hệ thống
	/// </summary>
	[Serializable]
	[Table("Setting")]
	public partial class Setting : BaseEntity, ILocalizedEntity
	{
		/// <summary>
		/// Tên thiết lập
		/// </summary>
		[Column, NotNull] public string Name  { get; set; } // varchar(200)
		/// <summary>
		/// Giá trị thiết lập
		/// </summary>
		[Column, NotNull] public string Value { get; set; } // varchar(6000)
		/// <summary>
		/// App (Ref: App)
		/// </summary>
		[Column, NotNull] public string AppId { get; set; } // varchar(36)
	}

	/// <summary>
	/// Danh sách Url tùy chỉnh, SEO
	/// </summary>
	[Serializable]
	[Table("UrlRecord")]
	public partial class UrlRecord : BaseEntity
	{
		/// <summary>
		/// Tên Entity
		/// </summary>
		[Column, NotNull] public string EntityName { get; set; } // varchar(255)
		/// <summary>
		/// Url tùy chỉnh, SEO
		/// </summary>
		[Column, NotNull] public string Slug       { get; set; } // varchar(400)
		/// <summary>
		/// Entity (Ref: BaseEntity)
		/// </summary>
		[Column, NotNull] public string EntityId   { get; set; } // varchar(36)
		/// <summary>
		/// Kích hoạt
		/// </summary>
		[Column, NotNull] public bool   IsActive   { get; set; } // tinyint(1)
		/// <summary>
		/// Ngôn ngữ (Ref: Language)
		/// </summary>
		[Column, NotNull] public string LanguageId { get; set; } // varchar(36)
	}

	[Serializable]
	[Table("UserRole")]
	public partial class UserRole : BaseEntity
	{
		[Column, NotNull] public string UserId { get; set; } // varchar(36)
		[Column, NotNull] public string RoleId { get; set; } // varchar(36)

		#region Associations

		/// <summary>
		/// FK_UserRoles_PK_Role
		/// </summary>
		[Association(ThisKey="RoleId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_UserRoles_PK_Role", BackReferenceName="UserRoles")]
		public Role Role { get; set; }

		#endregion
	}

	public static partial class TableExtensions
	{
		public static App Find(this ITable<App> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AppAction Find(this ITable<AppAction> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AppActionRole Find(this ITable<AppActionRole> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AppActionUserExclusion Find(this ITable<AppActionUserExclusion> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AppActionUserInclusion Find(this ITable<AppActionUserInclusion> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static AppMapping Find(this ITable<AppMapping> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Currency Find(this ITable<Currency> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Language Find(this ITable<Language> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static LocaleStringResource Find(this ITable<LocaleStringResource> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static LocalizedProperty Find(this ITable<LocalizedProperty> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Report Find(this ITable<Report> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Role Find(this ITable<Role> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Setting Find(this ITable<Setting> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UrlRecord Find(this ITable<UrlRecord> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UserRole Find(this ITable<UserRole> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}

#pragma warning restore 1591
