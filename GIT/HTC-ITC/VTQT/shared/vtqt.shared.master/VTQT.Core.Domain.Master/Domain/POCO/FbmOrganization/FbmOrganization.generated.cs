﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.Mapping;

using VTQT.Core.Domain.Apps;
using VTQT.Core.Domain.Localization;

namespace VTQT.Core.Domain.FbmOrganization
{
	/// <summary>
	/// Database       : ITC_FBM_Organizations
	/// Data Source    : 192.168.100.32
	/// Server Version : 5.7.33-36-57-log
	/// </summary>
	public partial class FbmOrganizationDataConnection : LinqToDB.Data.DataConnection
	{
		public ITable<__EFMigrationsHistory>        __EFMigrationsHistories       { get { return this.GetTable<__EFMigrationsHistory>(); } }
		public ITable<ConfigurationPersonalAccount> ConfigurationPersonalAccounts { get { return this.GetTable<ConfigurationPersonalAccount>(); } }
		public ITable<ConfigurationSystemParameter> ConfigurationSystemParameters { get { return this.GetTable<ConfigurationSystemParameter>(); } }
		public ITable<ContactInfo>                  ContactInfos                  { get { return this.GetTable<ContactInfo>(); } }
		public ITable<FCMToken>                     FCMTokens                     { get { return this.GetTable<FCMToken>(); } }
		public ITable<IntegrationEventLog>          IntegrationEventLogs          { get { return this.GetTable<IntegrationEventLog>(); } }
		public ITable<OrganizationUnit>             OrganizationUnits             { get { return this.GetTable<OrganizationUnit>(); } }
		public ITable<Otp>                          Otps                          { get { return this.GetTable<Otp>(); } }
		public ITable<Permission>                   Permissions                   { get { return this.GetTable<Permission>(); } }
		public ITable<Picture>                      Pictures                      { get { return this.GetTable<Picture>(); } }
		public ITable<Role>                         Roles                         { get { return this.GetTable<Role>(); } }
		public ITable<RolePermission>               RolePermissions               { get { return this.GetTable<RolePermission>(); } }
		public ITable<User>                         Users                         { get { return this.GetTable<User>(); } }
		public ITable<UserBankAccount>              UserBankAccounts              { get { return this.GetTable<UserBankAccount>(); } }
		public ITable<UserRole>                     UserRoles                     { get { return this.GetTable<UserRole>(); } }

		public FbmOrganizationDataConnection()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public FbmOrganizationDataConnection(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public FbmOrganizationDataConnection(LinqToDbConnectionOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public FbmOrganizationDataConnection(LinqToDbConnectionOptions<FbmOrganizationDataConnection> options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	[Serializable]
	[Table("__EFMigrationsHistory")]
	public partial class __EFMigrationsHistory : BaseIntEntity
	{
		[PrimaryKey, NotNull] public string MigrationId    { get; set; } // varchar(95)
		[Column,     NotNull] public string ProductVersion { get; set; } // varchar(32)
	}

	[Serializable]
	[Table("ConfigurationPersonalAccounts")]
	public partial class ConfigurationPersonalAccount : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture               { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate           { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate           { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy             { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy             { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive              { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted             { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder          { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath      { get; set; } // longtext
		[Column, NotNull    ] public int       UserId                { get; set; } // int(11)
		[Column, NotNull    ] public bool      AllowSendEmail        { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      AllowSendNotification { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      AllowSendSMS          { get; set; } // tinyint(1)

		#region Associations

		/// <summary>
		/// FK_ConfigurationPersonalAccounts_Users_UserId
		/// </summary>
		[Association(ThisKey="UserId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ConfigurationPersonalAccounts_Users_UserId", BackReferenceName="FK_ConfigurationPersonalAccounts_Users_UserId_BackReferences")]
		public User ConfigurationPersonalAccounts_Users_UserId { get; set; }

		#endregion
	}

	[Serializable]
	[Table("ConfigurationSystemParameters")]
	public partial class ConfigurationSystemParameter : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture                   { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate               { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate               { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy                 { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy                 { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive                  { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted                 { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder              { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath          { get; set; } // longtext
		[Column, NotNull    ] public int       ChangeRecordExportExcel   { get; set; } // int(11)
		[Column, NotNull    ] public int       ChangeRecordExportPdf     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationUnit          { get; set; } // longtext
		[Column,    Nullable] public string    Address                   { get; set; } // longtext
		[Column,    Nullable] public string    TaxCode                   { get; set; } // longtext
		[Column,    Nullable] public string    BankAccount               { get; set; } // longtext
		[Column,    Nullable] public string    TelephoneSwitchboard      { get; set; } // longtext
		[Column,    Nullable] public string    RepresentativePersonName  { get; set; } // longtext
		[Column,    Nullable] public string    RpPosition                { get; set; } // longtext
		[Column,    Nullable] public string    AuthorizationLetterNumber { get; set; } // longtext
		[Column,    Nullable] public string    TradingAddress            { get; set; } // longtext
		[Column,    Nullable] public string    Website                   { get; set; } // longtext
		[Column, NotNull    ] public int       NumberDaysBadDebt         { get; set; } // int(11)
		[Column, NotNull    ] public int       NumberDaysOverdue         { get; set; } // int(11)
	}

	[Serializable]
	[Table("ContactInfos")]
	public partial class ContactInfo : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column, NotNull    ] public int       UserId           { get; set; } // int(11)
		[Column,    Nullable] public string    Name             { get; set; } // varchar(512)
		[Column,    Nullable] public string    PhoneNumber      { get; set; } // varchar(10)
		[Column,    Nullable] public string    Email            { get; set; } // varchar(256)
		[Column,    Nullable] public string    Note             { get; set; } // varchar(1000)

		#region Associations

		/// <summary>
		/// FK_ContactInfos_Users_UserId
		/// </summary>
		[Association(ThisKey="UserId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ContactInfos_Users_UserId", BackReferenceName="FK_ContactInfos_Users_UserId_BackReferences")]
		public User ContactInfos_Users_UserId { get; set; }

		#endregion
	}

	[Serializable]
	[Table("FCMTokens")]
	public partial class FCMToken : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column,    Nullable] public string    ReceiverId       { get; set; } // longtext
		[Column,    Nullable] public string    Token            { get; set; } // longtext
		[Column,    Nullable] public string    Platform         { get; set; } // longtext
	}

	[Serializable]
	[Table("IntegrationEventLogs")]
	public partial class IntegrationEventLog : BaseIntEntity
	{
		[PrimaryKey, NotNull    ] public string   EventId       { get; set; } // char(36)
		[Column,     NotNull    ] public string   EventTypeName { get; set; } // longtext
		[Column,     NotNull    ] public int      State         { get; set; } // int(11)
		[Column,     NotNull    ] public int      TimesSent     { get; set; } // int(11)
		[Column,     NotNull    ] public DateTime CreationTime  { get; set; } // datetime(6)
		[Column,     NotNull    ] public string   Content       { get; set; } // longtext
		[Column,        Nullable] public string   TransactionId { get; set; } // longtext
	}

	[Serializable]
	[Table("OrganizationUnits")]
	public partial class OrganizationUnit : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column, NotNull    ] public string    IdentityGuid     { get; set; } // char(36)
		[Column,    Nullable] public string    Name             { get; set; } // varchar(256)
		[Column,    Nullable] public int?      ParentId         { get; set; } // int(11)
		[Column,    Nullable] public string    Code             { get; set; } // varchar(256)
		[Column,    Nullable] public string    ShortName        { get; set; } // varchar(256)
		[Column,    Nullable] public string    Address          { get; set; } // varchar(1000)
		[Column,    Nullable] public string    NumberPhone      { get; set; } // varchar(100)
		[Column,    Nullable] public int?      TypeId           { get; set; } // int(11)
		[Column,    Nullable] public string    Email            { get; set; } // varchar(256)
		[Column,    Nullable] public string    ProvinceId       { get; set; } // longtext
		[Column,    Nullable] public string    TreePath         { get; set; } // longtext

		#region Associations

		/// <summary>
		/// FK_Users_OrganizationUnits_OrganizationUnitId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="OrganizationUnitId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<User> FK_Users_OrganizationUnits_OrganizationUnitId_BackReferences { get; set; }

		#endregion
	}

	[Serializable]
	[Table("Otps")]
	public partial class Otp : BaseIntEntity
	{
		[Column(),         Nullable] public string    Culture          { get; set; } // longtext
		[Column(),      NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column(),         Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column(),         Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column(),         Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column(),      NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column(),      NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column(),      NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column(),         Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column(),         Nullable] public string    Phone            { get; set; } // longtext
		[Column("Otp"),    Nullable] public string    OtpColumn        { get; set; } // longtext
		[Column(),         Nullable] public DateTime? DateExpired      { get; set; } // datetime(6)
		[Column(),         Nullable] public bool?     IsUse            { get; set; } // tinyint(1)
	}

	[Serializable]
	[Table("Permissions")]
	public partial class Permission : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column, NotNull    ] public int       PermissionSetId  { get; set; } // int(11)
		[Column,    Nullable] public string    PermissionName   { get; set; } // varchar(256)
		[Column, NotNull    ] public string    PermissionCode   { get; set; } // varchar(256)
		[Column,    Nullable] public string    PermissionPage   { get; set; } // varchar(256)
		[Column,    Nullable] public string    Description      { get; set; } // varchar(1000)

		#region Associations

		/// <summary>
		/// FK_RolePermissions_Permissions_PermissionId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="PermissionId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<RolePermission> FK_RolePermissions_Permissions_PermissionId_BackReferences { get; set; }

		#endregion
	}

	[Serializable]
	[Table("Pictures")]
	public partial class Picture : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column,    Nullable] public string    Name             { get; set; } // varchar(256)
		[Column, NotNull    ] public string    FileName         { get; set; } // varchar(256)
		[Column,    Nullable] public string    FilePath         { get; set; } // longtext
		[Column, NotNull    ] public long      Size             { get; set; } // bigint(20)
		[Column,    Nullable] public int?      Order            { get; set; } // int(11)
		[Column, NotNull    ] public int       PictureType      { get; set; } // int(11)
		[Column, NotNull    ] public string    Extension        { get; set; } // longtext
		[Column,    Nullable] public string    RedirectLink     { get; set; } // longtext

		#region Associations

		/// <summary>
		/// FK_Users_Pictures_AvatarId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="AvatarId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<User> FK_Users_Pictures_AvatarId_BackReferences { get; set; }

		#endregion
	}

	[Serializable]
	[Table("Roles")]
	public partial class Role : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column, NotNull    ] public string    RoleName         { get; set; } // varchar(256)
		[Column, NotNull    ] public string    RoleCode         { get; set; } // varchar(256)
		[Column,    Nullable] public string    RoleDescription  { get; set; } // longtext

		#region Associations

		/// <summary>
		/// FK_RolePermissions_Roles_RoleId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="RoleId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<RolePermission> FK_RolePermissions_Roles_RoleId_BackReferences { get; set; }

		/// <summary>
		/// FK_UserRoles_Roles_RoleId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="RoleId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<UserRole> FK_UserRoles_Roles_RoleId_BackReferences { get; set; }

		#endregion
	}

	[Serializable]
	[Table("RolePermissions")]
	public partial class RolePermission : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column, NotNull    ] public int       RoleId           { get; set; } // int(11)
		[Column, NotNull    ] public int       PermissionId     { get; set; } // int(11)
		[Column, NotNull    ] public bool      Grant            { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      Deny             { get; set; } // tinyint(1)

		#region Associations

		/// <summary>
		/// FK_RolePermissions_Permissions_PermissionId
		/// </summary>
		[Association(ThisKey="PermissionId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_RolePermissions_Permissions_PermissionId", BackReferenceName="FK_RolePermissions_Permissions_PermissionId_BackReferences")]
		public Permission RolePermissions_Permissions_PermissionId { get; set; }

		/// <summary>
		/// FK_RolePermissions_Roles_RoleId
		/// </summary>
		[Association(ThisKey="RoleId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_RolePermissions_Roles_RoleId", BackReferenceName="FK_RolePermissions_Roles_RoleId_BackReferences")]
		public Role RolePermissions_Roles_RoleId { get; set; }

		#endregion
	}

	[Serializable]
	[Table("Users")]
	public partial class User : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture                     { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate                 { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate                 { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy                   { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy                   { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive                    { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted                   { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder                { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath            { get; set; } // longtext
		[Column, NotNull    ] public string    IdentityGuid                { get; set; } // longtext
		[Column,    Nullable] public string    AccountingCustomerCode      { get; set; } // varchar(256)
		[Column,    Nullable] public string    UserName                    { get; set; } // varchar(256)
		[Column,    Nullable] public string    Code                        { get; set; } // varchar(256)
		[Column,    Nullable] public string    FirstName                   { get; set; } // varchar(256)
		[Column,    Nullable] public string    LastName                    { get; set; } // varchar(256)
		[Column,    Nullable] public string    FullName                    { get; set; } // varchar(512)
		[Column,    Nullable] public string    ShortName                   { get; set; } // varchar(512)
		[Column,    Nullable] public string    MobilePhoneNo               { get; set; } // varchar(100)
		[Column,    Nullable] public string    Address                     { get; set; } // varchar(1000)
		[Column,    Nullable] public string    Email                       { get; set; } // longtext
		[Column,    Nullable] public string    IdNo                        { get; set; } // varchar(12)
		[Column,    Nullable] public DateTime? DateOfIssueID               { get; set; } // datetime
		[Column,    Nullable] public string    PlaceOfIssueID              { get; set; } // varchar(256)
		[Column,    Nullable] public string    LastIpAddress               { get; set; } // longtext
		[Column,    Nullable] public string    Password                    { get; set; } // longtext
		[Column,    Nullable] public string    SecurityStamp               { get; set; } // varchar(68)
		[Column,    Nullable] public int?      AvatarId                    { get; set; } // int(11)
		[Column,    Nullable] public int?      Status                      { get; set; } // int(11)
		[Column,    Nullable] public int?      Gender                      { get; set; } // int(11)
		[Column,    Nullable] public int?      OrganizationUnitId          { get; set; } // int(11)
		[Column,    Nullable] public string    FaxNo                       { get; set; } // varchar(50)
		[Column,    Nullable] public string    TaxIdNo                     { get; set; } // longtext
		[Column,    Nullable] public string    RepresentativePersonName    { get; set; } // varchar(256)
		[Column,    Nullable] public string    RpPhoneNo                   { get; set; } // varchar(256)
		[Column,    Nullable] public DateTime? RpDateOfBirth               { get; set; } // datetime(6)
		[Column,    Nullable] public string    RpJobPosition               { get; set; } // varchar(256)
		[Column,    Nullable] public string    BusinessRegCertificate      { get; set; } // varchar(256)
		[Column,    Nullable] public DateTime? BrcDateOfIssue              { get; set; } // datetime(6)
		[Column,    Nullable] public string    BrcIssuedBy                 { get; set; } // varchar(1000)
		[Column,    Nullable] public string    JobPosition                 { get; set; } // varchar(256)
		[Column,    Nullable] public string    JobTitle                    { get; set; } // varchar(256)
		[Column, NotNull    ] public bool      IsLock                      { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsPartner                   { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsEnterprise                { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsCustomer                  { get; set; } // tinyint(1)
		[Column,    Nullable] public string    ApplicationUserIdentityGuid { get; set; } // longtext
		[Column, NotNull    ] public bool      IsCustomerInternational     { get; set; } // tinyint(1)
		[Column,    Nullable] public string    Note                        { get; set; } // varchar(1000)
		[Column,    Nullable] public string    TradingAddress              { get; set; } // varchar(1000)

		#region Associations

		/// <summary>
		/// FK_ConfigurationPersonalAccounts_Users_UserId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="UserId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ConfigurationPersonalAccount> FK_ConfigurationPersonalAccounts_Users_UserId_BackReferences { get; set; }

		/// <summary>
		/// FK_ContactInfos_Users_UserId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="UserId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ContactInfo> FK_ContactInfos_Users_UserId_BackReferences { get; set; }

		/// <summary>
		/// FK_UserBankAccounts_Users_UserId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="UserId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<UserBankAccount> FK_UserBankAccounts_Users_UserId_BackReferences { get; set; }

		/// <summary>
		/// FK_UserRoles_Users_UserId_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="UserId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<UserRole> FK_UserRoles_Users_UserId_BackReferences { get; set; }

		/// <summary>
		/// FK_Users_OrganizationUnits_OrganizationUnitId
		/// </summary>
		[Association(ThisKey="OrganizationUnitId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Users_OrganizationUnits_OrganizationUnitId", BackReferenceName="FK_Users_OrganizationUnits_OrganizationUnitId_BackReferences")]
		public OrganizationUnit Users_OrganizationUnits_OrganizationUnitId { get; set; }

		/// <summary>
		/// FK_Users_Pictures_AvatarId
		/// </summary>
		[Association(ThisKey="AvatarId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Users_Pictures_AvatarId", BackReferenceName="FK_Users_Pictures_AvatarId_BackReferences")]
		public Picture Users_Pictures_AvatarId { get; set; }

		#endregion
	}

	[Serializable]
	[Table("UserBankAccounts")]
	public partial class UserBankAccount : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture           { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate       { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate       { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy         { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy         { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive          { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted         { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder      { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath  { get; set; } // longtext
		[Column,    Nullable] public string    BankName          { get; set; } // varchar(256)
		[Column,    Nullable] public string    BankAccountNumber { get; set; } // varchar(256)
		[Column,    Nullable] public string    BankBranch        { get; set; } // varchar(256)
		[Column, NotNull    ] public int       UserId            { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// FK_UserBankAccounts_Users_UserId
		/// </summary>
		[Association(ThisKey="UserId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_UserBankAccounts_Users_UserId", BackReferenceName="FK_UserBankAccounts_Users_UserId_BackReferences")]
		public User UserBankAccounts_Users_UserId { get; set; }

		#endregion
	}

	[Serializable]
	[Table("UserRoles")]
	public partial class UserRole : BaseIntEntity
	{
		[Column,    Nullable] public string    Culture          { get; set; } // longtext
		[Column, NotNull    ] public DateTime  CreatedDate      { get; set; } // datetime
		[Column,    Nullable] public DateTime? UpdatedDate      { get; set; } // datetime
		[Column,    Nullable] public string    CreatedBy        { get; set; } // longtext
		[Column,    Nullable] public string    UpdatedBy        { get; set; } // longtext
		[Column, NotNull    ] public bool      IsActive         { get; set; } // tinyint(1)
		[Column, NotNull    ] public bool      IsDeleted        { get; set; } // tinyint(1)
		[Column, NotNull    ] public int       DisplayOrder     { get; set; } // int(11)
		[Column,    Nullable] public string    OrganizationPath { get; set; } // longtext
		[Column, NotNull    ] public int       UserId           { get; set; } // int(11)
		[Column, NotNull    ] public int       RoleId           { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// FK_UserRoles_Roles_RoleId
		/// </summary>
		[Association(ThisKey="RoleId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_UserRoles_Roles_RoleId", BackReferenceName="FK_UserRoles_Roles_RoleId_BackReferences")]
		public Role UserRoles_Roles_RoleId { get; set; }

		/// <summary>
		/// FK_UserRoles_Users_UserId
		/// </summary>
		[Association(ThisKey="UserId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_UserRoles_Users_UserId", BackReferenceName="FK_UserRoles_Users_UserId_BackReferences")]
		public User UserRoles_Users_UserId { get; set; }

		#endregion
	}

	public static partial class FbmOrganizationDataConnectionStoredProcedures
	{
		#region GetPermissionOfUser

		public static IEnumerable<GetPermissionOfUserResult> GetPermissionOfUser(this FbmOrganizationDataConnection dataConnection, int? userId)
		{
			return dataConnection.QueryProc<GetPermissionOfUserResult>("`GetPermissionOfUser`",
				new DataParameter("userId", userId, LinqToDB.DataType.Int32));
		}

		public partial class GetPermissionOfUserResult
		{
			public int?   Id             { get; set; }
			public string PermissionCode { get; set; }
			public string PermissionName { get; set; }
			public string PermissionPage { get; set; }
			public string RoleName       { get; set; }
		}

		#endregion
	}

	public static partial class TableExtensions
	{
		public static __EFMigrationsHistory Find(this ITable<__EFMigrationsHistory> table, string MigrationId)
		{
			return table.FirstOrDefault(t =>
				t.MigrationId == MigrationId);
		}

		public static ConfigurationPersonalAccount Find(this ITable<ConfigurationPersonalAccount> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ConfigurationSystemParameter Find(this ITable<ConfigurationSystemParameter> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ContactInfo Find(this ITable<ContactInfo> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static FCMToken Find(this ITable<FCMToken> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static IntegrationEventLog Find(this ITable<IntegrationEventLog> table, string EventId)
		{
			return table.FirstOrDefault(t =>
				t.EventId == EventId);
		}

		public static OrganizationUnit Find(this ITable<OrganizationUnit> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Otp Find(this ITable<Otp> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Permission Find(this ITable<Permission> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Picture Find(this ITable<Picture> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Role Find(this ITable<Role> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static RolePermission Find(this ITable<RolePermission> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static User Find(this ITable<User> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UserBankAccount Find(this ITable<UserBankAccount> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UserRole Find(this ITable<UserRole> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}

#pragma warning restore 1591
