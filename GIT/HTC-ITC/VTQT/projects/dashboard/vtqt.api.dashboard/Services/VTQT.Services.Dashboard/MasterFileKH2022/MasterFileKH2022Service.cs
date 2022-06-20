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
    public class MasterFileKH2022Service : IMasterFileKH2022Service
    {
        private readonly IRepository<MasterFileKH2022> _repository;

        public MasterFileKH2022Service()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<MasterFileKH2022>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }


        public async Task<long> InsertAsync(IEnumerable<MasterFileKH2022> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<MasterFileKH2022> entity)
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

        public async Task<MasterFileKH2022> GetByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public IList<MasterFileKH2022> GetAll()
        {
            var query = from a in _repository.Table
                        select a;
            return query.ToList();
        }

        public async Task<IPagedList<MasterFileKH2022>> GetAllQuery(string key, int numberPage, IEnumerable<SelectListItem> listItem)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM MasterFileKH2022 f  where 1=1 ");
            if (!string.IsNullOrEmpty(key))
                sb.Append(" and MATCH(`Tuần báo cáo`,`Qua đại lý/ KH cuối`,`Khu vực`,`Danh mục khách hàng`,`Nhóm khách hàng`,`Loại hình cơ cấu Kh`,`Kiểu Khách hàng`,`Hạng Khách hàng`,`Tên Khách hàng`,`Mã khách hàng`,`Số hợp đồng`,`Ngày ký HĐ`,`Ngày nghiệm thu tính cước`,`Chi tiết triển khai CCDV`,`Băng thông quốc tế(M)`,`CID / User`) AGAINST(@key)   ");
            foreach (var item in listItem)
            {
                if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                    sb.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
            }
            sb.Append(" LIMIT 100 OFFSET @numberPage ");

            //
            StringBuilder sbCount = new StringBuilder();
            sbCount.Append("SELECT COUNT(*) FROM ( ");
            sbCount.Append("SELECT Id FROM MasterFileKH2022 f  where 1=1 ");
            if (!string.IsNullOrEmpty(key))
                sbCount.Append(" and MATCH(`Tuần báo cáo`,`Qua đại lý/ KH cuối`,`Khu vực`,`Danh mục khách hàng`,`Nhóm khách hàng`,`Loại hình cơ cấu Kh`,`Kiểu Khách hàng`,`Hạng Khách hàng`,`Tên Khách hàng`,`Mã khách hàng`,`Số hợp đồng`,`Ngày ký HĐ`,`Ngày nghiệm thu tính cước`,`Chi tiết triển khai CCDV`,`Băng thông quốc tế(M)`,`CID / User`) AGAINST(@key)   ");
            foreach (var item in listItem)
            {
                if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.Text))
                    sbCount.Append(" and f." + item.Value + " like '%" + item.Text + "%' ");
            }
            sbCount.Append(" ) t   ");
            DataParameter pkey = new DataParameter("@key", key);
            DataParameter pNumber = new DataParameter("@numberPage", numberPage * 100);

            var res = await _repository.DataConnection.QueryToListAsync<MasterFileKH2022>(sb.ToString(), pkey, pNumber);
            var resCount = await _repository.DataConnection.QueryToListAsync<int>(sbCount.ToString(), pkey);
            return new PagedList<MasterFileKH2022>(res, numberPage, 100, resCount.FirstOrDefault());

        }

        public async Task<IList<MasterFileKH2022>> GetObject()
        {
            var sql =
                "SELECT * from MasterFileKH2022 f order by f.`Tuần báo cáo` desc LIMIT 1 OFFSET 0 ";
            var res = await _repository.DataConnection.QueryToListAsync<MasterFileKH2022>(sql);
            return res;
        }

        public async Task<int> GetCountQuery()
        {
            return await _repository.Table.CountAsync();
        }
    }
}