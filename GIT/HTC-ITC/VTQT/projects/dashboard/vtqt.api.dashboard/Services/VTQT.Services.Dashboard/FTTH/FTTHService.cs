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
    public class FTTHService : IFTTHService
    {
        private readonly IRepository<FTTH> _repository;
        private readonly IRepository<StorageValue> _repositoryStorageValue;


        public FTTHService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<FTTH>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
            _repositoryStorageValue =
 EngineContext.Current.Resolve<IRepository<StorageValue>>(DataConnectionHelper.ConnectionStringNames
     .Dashboard);
        }


        public async Task<long> InsertAsync(IEnumerable<FTTH> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<FTTH> entity)
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

        public async Task<FTTH> GetByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public IList<FTTH> GetAll()
        {
            var query = from a in _repository.Table
                        select a;
            return query.ToList();
        }

        /// <summary>
        /// search theo full text hoặc theo cột
        /// </summary>
        /// <param name="key"></param>
        /// <param name="numberPage"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        public async Task<IPagedList<FTTH>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem, string idStorageValue)
        {
            if (string.IsNullOrEmpty(idStorageValue))
            {
                throw new ArgumentException($"'{nameof(idStorageValue)}' cannot be null or empty.", nameof(idStorageValue));
            }

            var check = await _repositoryStorageValue.GetByIdAsync(idStorageValue);
            if (check == null)
                return new PagedList<FTTH>(null, numberPage, 100, 0);
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCount = new StringBuilder();

            if (check.ActiveGetAllData == true && check.OptionSelectColumn != null && check.OptionSelectColumn.Split(',') != null && check.OptionSelectColumn.Split(',').Length > 0)
            {
                sb.Append("select Id, " + check.OptionSelectColumn + " from FTTH");
                var resAll = await _repository.DataConnection.QueryToListAsync<FTTH>(sb.ToString());
                return new PagedList<FTTH>(resAll, numberPage, 100, 0);

            }
            else if (check.ActiveGetAllData == true && string.IsNullOrEmpty(check.OptionSelectColumn))
            {
                sb.Append("select * from FTTH");
                var resAll = await _repository.DataConnection.QueryToListAsync<FTTH>(sb.ToString());
                return new PagedList<FTTH>(resAll, numberPage, 100, 0);

            }
            else if (check.ActiveGetAllData == false && check.OptionSelectColumn != null && check.OptionSelectColumn != null && check.OptionSelectColumn.Split(',') != null && check.OptionSelectColumn.Split(',').Length > 0)
            {
                sb.Append("select Id, " + check.OptionSelectColumn + " from FTTH f  where 1=1 ");
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
                sbCount.Append("SELECT Id FROM FTTH f  where 1=1 ");
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

                var res = await _repository.DataConnection.QueryToListAsync<FTTH>(sb.ToString(), pkey, pNumber);
                var resCount = await _repository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), pkey);
                return new PagedList<FTTH>(res, numberPage, 100, resCount.FirstOrDefault());
            }
            else if (check.ActiveGetAllData == false && string.IsNullOrEmpty(check.OptionSelectColumn))
            {
                sb.Append("select * from FTTH f  where 1=1 ");
                if (!string.IsNullOrEmpty(key))
                    sb.Append(" and MATCH(`Tỉnh thành`,`Tên dự án`,`Tình trạng hoạt động`,`Loại khách hàng`,`Khách hàng`,`Số điện thoại`,`Địa chỉ`,`Mã KH/CID`,`Số Hợp đồng`,`CS Phụ trách`,`Gói cước`,`Chủ thể xuất hóa đơn`,`Thanh lý (trước hạn hay sau hạn)`,`Chi tiết lý do thanh lý`,`Phân loại thanh lý`) AGAINST(@key)   ");
                foreach (var item in listItem)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                        sb.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
                }
                sb.Append(" LIMIT 100 OFFSET @numberPage ");

                //
                sbCount.Append("SELECT COUNT(*) FROM ( ");
                sbCount.Append("SELECT Id FROM FTTH f  where 1=1 ");
                if (!string.IsNullOrEmpty(key))
                    sb.Append(" and MATCH(`Tỉnh thành`,`Tên dự án`,`Tình trạng hoạt động`,`Loại khách hàng`,`Khách hàng`,`Số điện thoại`,`Địa chỉ`,`Mã KH/CID`,`Số Hợp đồng`,`CS Phụ trách`,`Gói cước`,`Chủ thể xuất hóa đơn`,`Thanh lý (trước hạn hay sau hạn)`,`Chi tiết lý do thanh lý`,`Phân loại thanh lý`) AGAINST(@key)   ");
                foreach (var item in listItem)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                        sbCount.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
                }
                sbCount.Append(" ) t   ");
                DataParameter pkey = new DataParameter("@key", key);
                DataParameter pNumber = new DataParameter("@numberPage", numberPage * 100);

                var res = await _repository.DataConnection.QueryToListAsync<FTTH>(sb.ToString(), pkey, pNumber);
                var resCount = await _repository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), pkey);
                return new PagedList<FTTH>(res, numberPage, 100, resCount.FirstOrDefault());
            }

            return new PagedList<FTTH>(null, numberPage, 100, 0);


        }

        public async Task<IList<FTTH>> GetObject()
        {
            var sql =
                "SELECT * from FTTH f order by f.`Loại khách hàng` desc LIMIT 1 OFFSET 0 ";
            var res = await _repository.DataConnection.QueryToListAsync<FTTH>(sql);
            return res;
        }

        public async Task<int> GetCountQuery()
        {
            return await _repository.Table.CountAsync();
        }

        public async Task<IList<FTTH>> GetList()
        {
            var sql = "SELECT f.`Tên dự án`,f.`Khách hàng`,f.`Cước HTC được nhận được TB sau khi trừ đi khuyến mại`,f.`Tình trạng hoạt động`,f.`Ngày thanh lý`,f.`Ngày nghiệm thu` FROM FTTH2022 f";

            var res = await _repository.DataConnection.QueryToListAsync<FTTH>(sql);
            return res;
        }
    }
}