using LinqToDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core.Data;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB.Data;
using VTQT.Core.Domain.FbmOrganization;

namespace VTQT.Services.Asset
{
    public partial class AuditDetailService : IAuditDetailService
    {
        #region Fields
        private readonly IRepository<Audit> _auditRepository;
        private readonly IRepository<AuditDetail> _auditDetailRepository;
        private readonly IRepository<Core.Domain.Asset.Asset> _assetItemRepository;
        private readonly IRepository<AssetCategory> _assetCategoryRepository;
        private readonly IIntRepository<OrganizationUnit> _organizationRepository;
        #endregion

        #region Ctor
        public AuditDetailService()
        {
            _auditRepository = EngineContext.Current.Resolve<IRepository<Audit>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _auditDetailRepository = EngineContext.Current.Resolve<IRepository<AuditDetail>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _assetItemRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.Asset.Asset>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _assetCategoryRepository = EngineContext.Current.Resolve<IRepository<AssetCategory>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _organizationRepository = EngineContext.Current.Resolve<IIntRepository<OrganizationUnit>>(DataConnectionHelper.ConnectionStringNames.FbmOrganization);

        }
        #endregion

        #region Methods

        public async Task InsertAsync(AuditDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditDetailRepository.DataConnection;

                await db.InsertAsync(entity);

                if (entity.FK_AuditDetail_Id_BackReferences != null && entity.FK_AuditDetail_Id_BackReferences.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.FK_AuditDetail_Id_BackReferences);
                }

                transaction.Complete();
            }
        }

        public async Task UpdateAsync(AuditDetail entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = TransactionConfig.IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = _auditDetailRepository.DataConnection;

                await db.UpdateAsync(entity);

                await db.GetTable<AuditDetailSerial>()
                    .Where(w => w.AuditDetailId == entity.Id)
                    .DeleteAsync();

                if (entity.FK_AuditDetail_Id_BackReferences != null && entity.FK_AuditDetail_Id_BackReferences.Any())
                {
                    var bulkSerialResult = await db.BulkCopyAsync(new BulkCopyOptions(), entity.FK_AuditDetail_Id_BackReferences);
                }
                transaction.Complete();
            }
        }

        public async Task DeletesAsync(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            await _auditDetailRepository.DeleteAsync(ids);
        }

        public virtual IList<AuditDetail> GetByAuditId(AuditDetailSearchContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.AuditId))
                throw new ArgumentNullException(nameof(ctx.AuditId));

            var query =
                from x in _auditDetailRepository.Table
                join i in _assetItemRepository.Table on x.ItemId equals i.Id into ij
                from i in ij
                where x.AuditId == ctx.AuditId
                orderby i.Code
                select x;

            return query.ToList();
        }

        public async Task<AuditDetail> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _auditDetailRepository.GetByIdAsync(id);

            return result;
        }

        public async Task<bool> ExistsAsync(string itemId, string auditId)
        {
            return await _auditDetailRepository.Table
                .AnyAsync(x => !string.IsNullOrEmpty(x.ItemId)
                && x.ItemId.Equals(itemId)
                && !string.IsNullOrEmpty(x.AuditId)
                && x.AuditId.Equals(auditId));
        }

        public IQueryable<AuditDetail> GetListById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = from p in _auditDetailRepository.Table where p.AuditId.Equals(id.Trim()) select p;

            return result;
        }

        public IQueryable<AuditDetail> GetListShowNameById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = from i in _auditDetailRepository.Table
                        join v in _assetItemRepository.Table on i.ItemId equals v.Id into vi
                        from iv in vi.DefaultIfEmpty()
                        select new AuditDetail
                        {
                            Id = i.Id,
                            ItemId = i.ItemId,
                            AuditId = i.AuditId,
                            Quantity = i.Quantity,
                            AuditQuantity = i.AuditQuantity,
                            Conclude = i.Conclude,

                            Asset = iv == null ? null : new Core.Domain.Asset.Asset
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name
                            }
                        };
            var result = from p in query where p.AuditId.Equals(id.Trim()) select p;

            return result;
        }

        public async Task DeletesAsync(IEnumerable<AuditDetail> auditDetails)
        {
            if (auditDetails == null)
            {
                throw new ArgumentNullException(nameof(auditDetails));
            }

            await _auditDetailRepository.DeleteAsync(auditDetails);
        }

        public async Task<IList<AuditDetail>> GetAuditDetailByAssetItemId(string idOrganization, string idAssetItem)
        {
            if (string.IsNullOrEmpty(idOrganization) && string.IsNullOrEmpty(idAssetItem))
                throw new NotImplementedException("Not Id to Search Table");

            // BuildMyString.com generated code. Please enjoy your string responsibly.



            StringBuilder sb = new StringBuilder();




            DataParameter p1 = new DataParameter("pItem", idAssetItem);
            DataParameter p2 = new DataParameter("pW", idOrganization);
            return await _auditDetailRepository.DataConnection.QueryToListAsync<AuditDetail>(sb.ToString(), p1, p2);
        }

        public IList<Core.Domain.Asset.Asset> GetAuditDetailByOrganizationUnitId(string idOrganizationUnit, DateTime dateTime, int assetType)
        {
            if (string.IsNullOrEmpty(idOrganizationUnit))
                throw new NotImplementedException(nameof(idOrganizationUnit));
            var query = from a in _assetItemRepository.Table
                        where a.AllocationDate <= dateTime
                        where a.AssetType.Equals(assetType)
                        select a;

            var department = _organizationRepository.Table.FirstOrDefault(x => x.Id.ToString() == idOrganizationUnit);

            if (!string.IsNullOrEmpty(department.TreePath))
            {
                var departmentIds = _organizationRepository.Table.Where(x => !string.IsNullOrEmpty(x.TreePath) &&
                                                             x.TreePath.Contains(department.TreePath))
                                                       .Select(x => x.Id.ToString());
                if (departmentIds?.ToList().Count > 0)
                {
                    var listId = departmentIds.ToList();

                    query = query.Where(x => listId.Contains(x.OrganizationUnitId));
                }
            }
            query = from a in query
                    select new Core.Domain.Asset.Asset
                    {
                        WareHouseItemCode = a.Id,
                        UnitName = a.UnitName,
                        Name = a.Name,
                        OriginQuantity = a.OriginQuantity - a.RecallQuantity - a.SoldQuantity - a.BrokenQuantity
                    };

            return query.ToList();
        }

        public IList<Core.Domain.Asset.Asset> GetAuditDetailByStationCode(string idStationCode, DateTime dateTime, int assetType)
        {

            if (string.IsNullOrEmpty(idStationCode))
                throw new NotImplementedException(nameof(idStationCode));
            var result = from a in _assetItemRepository.Table
                         where a.StationCode.Equals(idStationCode) && a.AllocationDate <= dateTime
                         where a.AssetType.Equals(assetType)
                         select new Core.Domain.Asset.Asset
                         {
                             WareHouseItemCode = a.Id,
                             UnitName = a.UnitName,
                             Name = a.Name,
                             OriginQuantity = a.OriginQuantity - a.RecallQuantity - a.SoldQuantity - a.BrokenQuantity
                         };
            return result.ToList();

        }

        public IList<Core.Domain.Asset.Asset> GetAuditDetailByProjectCode(string idProjectCode, DateTime dateTime, int assetType)
        {
            if (string.IsNullOrEmpty(idProjectCode))
                throw new NotImplementedException(nameof(idProjectCode));
            var result = from a in _assetItemRepository.Table
                         where a.ProjectCode.Equals(idProjectCode) && a.AllocationDate <= dateTime
                         where a.AssetType.Equals(assetType)
                         select new Core.Domain.Asset.Asset
                         {
                             WareHouseItemCode = a.Id,
                             Name = a.Name,
                             UnitName = a.UnitName,
                             OriginQuantity = a.OriginQuantity - a.RecallQuantity - a.SoldQuantity - a.BrokenQuantity
                         };
            return result.ToList();

        }

        public virtual async Task<long> InsertRangeAsync(IEnumerable<AuditDetail> entities)
        {
            try
            {
                if (entities is null)
                    throw new ArgumentNullException(nameof(entities));
                var result = await _auditDetailRepository.UpdateAsync(entities);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        #endregion
    }
}
