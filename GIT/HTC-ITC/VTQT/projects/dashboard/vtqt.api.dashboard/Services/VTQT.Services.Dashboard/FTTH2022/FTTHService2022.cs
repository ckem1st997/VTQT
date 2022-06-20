using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB.Data;
using System.Text;
using VTQT.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VTQT.Services.Dashboard
{
    public class FTTHService2022 : IFTTHService2022
    {
        private readonly IRepository<FTTH2022> _repository;
        private readonly IRepository<StorageValue> _repositoryStorageValue;

        public FTTHService2022()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<FTTH2022>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
            _repositoryStorageValue =
    EngineContext.Current.Resolve<IRepository<StorageValue>>(DataConnectionHelper.ConnectionStringNames
        .Dashboard);
        }


        public async Task<long> InsertAsync(IEnumerable<FTTH2022> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<FTTH2022> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.UpdateAsync(entity);
            return res;
        }

        public async Task<long> DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var res = await _repository.DeleteAsync(ids);
            return res;
        }

        public async Task<FTTH2022> GetByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public IList<FTTH2022> GetAll()
        {
            var query = from a in _repository.Table
                        select a;
            return query.ToList();
        }

        public async Task<IPagedList<FTTH2022>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem, string idStorageValue)
        {
            if (string.IsNullOrEmpty(idStorageValue))
            {
                throw new ArgumentException($"'{nameof(idStorageValue)}' cannot be null or empty.", nameof(idStorageValue));
            }

            var check = await _repositoryStorageValue.GetByIdAsync(idStorageValue);
            if (check == null)
                return new PagedList<FTTH2022>(null, numberPage, 100, 0);
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCount = new StringBuilder();

            if (check.ActiveGetAllData == true && check.OptionSelectColumn != null && check.OptionSelectColumn.Split(',') != null && check.OptionSelectColumn.Split(',').Length > 0)
            {
                sb.Append("select Id, " + check.OptionSelectColumn + " from FTTH2022");
                var resAll = await _repository.DataConnection.QueryToListAsync<FTTH2022>(sb.ToString());
                return new PagedList<FTTH2022>(resAll, numberPage, 100, 0);

            }
            else if (check.ActiveGetAllData == true && string.IsNullOrEmpty(check.OptionSelectColumn))
            {
                sb.Append("select * from FTTH2022");
                var resAll = await _repository.DataConnection.QueryToListAsync<FTTH2022>(sb.ToString());
                return new PagedList<FTTH2022>(resAll, numberPage, 100, 0);

            }
            else if (check.ActiveGetAllData == false && check.OptionSelectColumn != null && check.OptionSelectColumn!=null&& check.OptionSelectColumn.Split(',') != null && check.OptionSelectColumn.Split(',').Length > 0)
            {
                sb.Append("select Id, " + check.OptionSelectColumn + " from FTTH2022 f  where 1=1 ");
                if (!string.IsNullOrEmpty(key))
                    sb.Append(" and MATCH(`Tuần cập nhật`,Miền,`Tỉnh thành`,`Tên dự án`,`Tình trạng hoạt động`,`Loại khách hàng`,`Khách hàng`,`Số điện thoại`,`Địa chỉ`,`Mã KH/CID`,`Số Hợp đồng`,`CS Phụ trách`,`Sale phụ trách`,`Gói cước`) AGAINST(@key)   ");
                foreach (var item in listItem)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                        sb.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
                }
                sb.Append(" LIMIT 100 OFFSET @numberPage ");

                //
                sbCount.Append("SELECT COUNT(*) FROM ( ");
                sbCount.Append("SELECT Id FROM FTTH2022 f  where 1=1 ");
                if (!string.IsNullOrEmpty(key))
                    sbCount.Append(" and MATCH(`Tuần cập nhật`,Miền,`Tỉnh thành`,`Tên dự án`,`Tình trạng hoạt động`,`Loại khách hàng`,`Khách hàng`,`Số điện thoại`,`Địa chỉ`,`Mã KH/CID`,`Số Hợp đồng`,`CS Phụ trách`,`Sale phụ trách`,`Gói cước`) AGAINST(@key)   ");
                foreach (var item in listItem)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                        sbCount.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
                }
                sbCount.Append(" ) t   ");
                DataParameter pkey = new DataParameter("@key", key);
                DataParameter pNumber = new DataParameter("@numberPage", numberPage * 100);

                var res = await _repository.DataConnection.QueryToListAsync<FTTH2022>(sb.ToString(), pkey, pNumber);
                var resCount = await _repository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), pkey);
                return new PagedList<FTTH2022>(res, numberPage, 100, resCount.FirstOrDefault());
            }
            else if (check.ActiveGetAllData == false &&  string.IsNullOrEmpty(check.OptionSelectColumn) )
            {
                sb.Append("select * from FTTH2022 f  where 1=1 ");
                if (!string.IsNullOrEmpty(key))
                    sb.Append(" and MATCH(`Tuần cập nhật`,Miền,`Tỉnh thành`,`Tên dự án`,`Tình trạng hoạt động`,`Loại khách hàng`,`Khách hàng`,`Số điện thoại`,`Địa chỉ`,`Mã KH/CID`,`Số Hợp đồng`,`CS Phụ trách`,`Sale phụ trách`,`Gói cước`) AGAINST(@key)   ");
                foreach (var item in listItem)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                        sb.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
                }
                sb.Append(" LIMIT 100 OFFSET @numberPage ");

                //
                sbCount.Append("SELECT COUNT(*) FROM ( ");
                sbCount.Append("SELECT Id FROM FTTH2022 f  where 1=1 ");
                if (!string.IsNullOrEmpty(key))
                    sbCount.Append(" and MATCH(`Tuần cập nhật`,Miền,`Tỉnh thành`,`Tên dự án`,`Tình trạng hoạt động`,`Loại khách hàng`,`Khách hàng`,`Số điện thoại`,`Địa chỉ`,`Mã KH/CID`,`Số Hợp đồng`,`CS Phụ trách`,`Sale phụ trách`,`Gói cước`) AGAINST(@key)   ");
                foreach (var item in listItem)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                        sbCount.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
                }
                sbCount.Append(" ) t   ");
                DataParameter pkey = new DataParameter("@key", key);
                DataParameter pNumber = new DataParameter("@numberPage", numberPage * 100);

                var res = await _repository.DataConnection.QueryToListAsync<FTTH2022>(sb.ToString(), pkey, pNumber);
                var resCount = await _repository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), pkey);
                return new PagedList<FTTH2022>(res, numberPage, 100, resCount.FirstOrDefault());
            }

            return new PagedList<FTTH2022>(null, numberPage, 100, 0);
        }

        public async Task<IList<FTTH2022>> GetObject()
        {
            var sql =
                "SELECT * from FTTH f order by f.`Loại khách hàng` desc LIMIT 1 OFFSET 0 ";
            var res = await _repository.DataConnection.QueryToListAsync<FTTH2022>(sql);
            return res;
        }

        public async Task<int> GetCountQuery()
        {
            return await _repository.Table.CountAsync();
        }

        public async Task<IList<FTTH2022>> GetList()
        {
            var sql = "SELECT f.`Tên dự án`,f.`Khách hàng`,f.`Cước HTC được nhận được TB sau khi trừ đi khuyến mại`,f.`Tình trạng hoạt động`,f.`Ngày thanh lý`,f.`Ngày nghiệm thu` FROM FTTH2022 f";

            var res = await _repository.DataConnection.QueryToListAsync<FTTH2022>(sql);
            return res;
        }
    }
}