using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Caching;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Core.Domain.Warehouse;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace VTQT.Services.Warehouse
{
    public partial class Print : IPrint
    {

        #region Fields

        private readonly IRepository<Outward> _repository;
        private readonly IRepository<WareHouse> _warehouseRepository;
        private readonly IRepository<Inward> _inwardRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Unit> _unitRepository;
        private readonly IRepository<WareHouseItem> _wareHouseItemRepository;
        private readonly IRepository<Audit> _auditRepository;

        #endregion

        #region Ctor

        public Print()
        {
            _repository = EngineContext.Current.Resolve<IRepository<Outward>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _warehouseRepository = EngineContext.Current.Resolve<IRepository<WareHouse>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _inwardRepository = EngineContext.Current.Resolve<IRepository<Inward>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
            _auditRepository = EngineContext.Current.Resolve<IRepository<Audit>>(DataConnectionHelper.ConnectionStringNames.Warehouse);
        }

        #endregion

        #region Methods

        public async Task<Outward> GetByIdToWordOutAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var wareHouse = from i in _warehouseRepository.Table select i;
            var wareHouseTo = from i in _warehouseRepository.Table select i;
            var query = from i in _repository.Table
                        join v in _warehouseRepository.Table on i.WareHouseID equals v.Id into vi
                        join c in _warehouseRepository.Table on i.ToWareHouseId equals c.Id into ic
                        where i.Id.Equals(id)
                        from iv in vi.DefaultIfEmpty()
                        from ci in ic.DefaultIfEmpty()
                        select new Outward
                        {
                            Id = i.Id,
                            VoucherCodeReality=i.VoucherCodeReality,
                            VoucherCode = i.VoucherCode,
                            VoucherDate = i.VoucherDate,
                            Deliver = i.Deliver,
                            Receiver = i.Receiver,
                            Reason = i.Reason,
                            ReasonDescription = i.ReasonDescription,
                            Description = i.Description,
                            CreatedBy = i.CreatedBy,
                            CreatedDate = i.CreatedDate,
                            DeliverAddress=i.DeliverAddress,
                            DeliverDepartment=i.DeliverDepartment,
                            DeliverPhone=i.DeliverPhone,
                            ReceiverAddress=i.ReceiverAddress,
                            ReceiverPhone=i.ReceiverPhone,
                            ReceiverDepartment=i.ReceiverDepartment,


                            WareHouse = iv == null ? null : new WareHouse
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name,
                                Address=iv.Address
                            },

                            ToWareHouse = ci == null ? null : new WareHouse
                            {
                                Id = ci.Id,
                                Code = ci.Code,
                                Name = ci.Name,
                                Address = ci.Address

                            }
                        };
            return query.FirstOrDefault();
        }


        public async Task<Inward> GetByIdToWordInAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = from i in _inwardRepository.Table
                        join u in _vendorRepository.Table on i.VendorId equals u.Id into ui
                        join v in _warehouseRepository.Table on i.WareHouseID equals v.Id into vi
                        from iu in ui.DefaultIfEmpty()
                        from iv in vi.DefaultIfEmpty()
                        where i.Id.Equals(id)
                        select new Inward
                        {
                            Id = i.Id,
                            WareHouseID = i.WareHouseID,
                            VendorId = i.VendorId,
                            Reference = i.Reference,
                            Voucher=i.Voucher,
                            VoucherCode = i.VoucherCode,
                            VoucherDate = i.VoucherDate,
                            Deliver = i.Deliver,
                            Description = i.Description,
                            Reason = i.Reason,
                            ReasonDescription = i.ReasonDescription,
                            Receiver = i.Receiver,
                            CreatedBy = i.CreatedBy,
                            CreatedDate = i.CreatedDate,
                            ModifiedBy = i.ModifiedBy,
                            ModifiedDate = i.ModifiedDate,
                            DeliverAddress = i.DeliverAddress,
                            DeliverDepartment = i.DeliverDepartment,
                            DeliverPhone = i.DeliverPhone,
                            ReceiverAddress = i.ReceiverAddress,
                            ReceiverPhone = i.ReceiverPhone,
                            ReceiverDepartment = i.ReceiverDepartment,

                            Vendor = iu == null ? null : new Vendor
                            {
                                Id = iu.Id,
                                Name = iu.Name
                            },
                            WareHouse = iv == null ? null : new WareHouse
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name,
                                Address = iv.Address
                            }
                        };
            return query.FirstOrDefault();
        }

        public async Task<IList<OutwardDetail>> GetByIdOutDetailsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            // BuildMyString.com generated code. Please enjoy your string responsibly.

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append("  whi.Name as ItemId, ");
            sb.Append("  od.Quantity, ");
            sb.Append("  u.UnitName AS UnitId ");
            sb.Append("FROM OutwardDetail od ");
            sb.Append("  INNER JOIN WareHouseItem whi ");
            sb.Append("    ON od.ItemId = whi.Id ");
            sb.Append("  INNER JOIN Unit u ");
            sb.Append("    ON od.UnitId = u.Id ");
            sb.Append("  INNER JOIN Outward o ");
            sb.Append("    ON od.OutwardId = o.Id ");
            sb.Append("WHERE o.Id = @p1 ");


            DataParameter p1 = new DataParameter("p1", id);

            return await _inwardRepository.DataConnection.QueryToListAsync<OutwardDetail>(sb.ToString(), p1);
        }

        public async Task<Audit> GetByIdToWordAuditAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var wareHouse = from i in _warehouseRepository.Table select i;
            var query = from i in _auditRepository.Table
                        join v in _warehouseRepository.Table on i.WareHouseId equals v.Id into vi
                        where i.Id.Equals(id)
                        from iv in vi.DefaultIfEmpty()
                        select new Audit
                        {
                            Id = i.Id,
                            VoucherCode = i.VoucherCode,
                            VoucherDate = i.VoucherDate,
                            Description = i.Description,
                            CreatedBy = i.CreatedBy,
                            CreatedDate = i.CreatedDate,
                            ModifiedDate = i.ModifiedDate,

                            WareHouse = iv == null ? null : new WareHouse
                            {
                                Id = iv.Id,
                                Code = iv.Code,
                                Name = iv.Name,
                                Address = iv.Address
                            },
                        };
            return query.FirstOrDefault();
        }

        #endregion
    }
}

