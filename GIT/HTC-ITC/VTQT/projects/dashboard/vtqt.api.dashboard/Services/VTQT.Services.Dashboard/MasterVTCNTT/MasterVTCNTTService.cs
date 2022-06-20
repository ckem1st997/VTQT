using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Dashboard;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using LinqToDB.Data;

namespace VTQT.Services.Dashboard
{
    public class MasterVTCNTTService : IMasterVTCNTTService
    {
        private readonly IRepository<MasterVTCNTT> _repository;
        private IMasterVTCNTTService _masterVtcnttServiceImplementation;

        public MasterVTCNTTService()
        {
            _repository =
                EngineContext.Current.Resolve<IRepository<MasterVTCNTT>>(DataConnectionHelper.ConnectionStringNames
                    .Dashboard);
        }


        public async Task<long> InsertAsync(IEnumerable<MasterVTCNTT> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        public async Task<long> UpdateAsync(IEnumerable<MasterVTCNTT> entity)
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

        public async Task<MasterVTCNTT> GetByIdAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public IList<MasterVTCNTT> GetAll()
        {
            var query = from a in _repository.Table
                select a;
            return query.ToList();
        }

        public async Task<IList<MasterVTCNTT>> GetAllQuery()
        {
            var sql =
              //  "SELECT `Tuần báo cáo`, `KH nền -0 hay mới - 1`, `Qua đại lý/ KH cuối`, `Khu vực`, `Danh mục khách hàng`, `Nhóm khách hàng`, `Loại hình cơ cấu Kh`, `Kiểu Khách hàng`, `Hạng Khách hàng`, `Tên Khách hàng`, `Mã khách hàng`, `Số hợp đồng`, `Ngày ký HĐ`, `Năm NT tính cước`, `Ngày ký HĐ (nâng cấp)`, `Ngày nghiệm thu tính cước (nâng cấp)`, `Chi tiết triển khai CCDV`, `Tình trạng CCDV`, `Ngày thanh lý/giảm giá`, `Lý do thanh lý/giảm giá/tạm ngưng`, `Phân loại thanh lý/giảm giá/tạm ngưng`, `Tình trạng HĐ`, `Loại trừ KPI`, `Thời hạn HĐ`, `Loại HĐ (1 lần hay hàng tháng)`, `CID / User`, `Điểm đầu`, `Điểm cuối`, `Chi tiết dịch vụ`, `Băng thông trong nước (M)`, `Băng thông quốc tế (M)`, `Phạm vi kênh`, `Loại doanh thu`, `Loại dịch vụ`, `Tranding (Yes/No)`, `cước cài đặt (USD)`, `cước cài đặt (VNĐ)`, `Cước hàng tháng (USD) 2020`, `Cước hàng tháng (VNĐ) (trước VAT) 2020`, `Cước hàng tháng (USD) 2021`, `Cước hàng tháng (VNĐ) (trước VAT) 2021`, `Chi phí đầu vào (cước tháng)`, `Số hợp đồng đầu vào liên quan`, `Đối tác/Tỷ lệ phân chia DT`, `Tỷ lệ phân chí doanh thu`, `Tỷ lệ hoa hồng`, `Ghi chú`, `CS phụ trách`, `KD phụ trách`, `Bộ phận kí HĐ`, `Ban QL KH`, `Nền 2020 chuyển sang 2021 chốt tại thời điểm 31/12/2020`, `Tháng 1 - GT Phát triển mới`, `Tháng 2 - GT Phát triển mới`, `Tháng 3 - GT Phát triển mới`, `Tháng 4 - GT Phát triển mới`, `Tháng 5 - GT Phát triển mới`, `Tháng 6 - GT Phát triển mới`, `Tháng 7 - GT Phát triển mới`, `Tháng 8 - GT Phát triển mới`, `Tháng 9 - GT Phát triển mới`, `Tháng 10 - GT Phát triển mới`, `Tháng 11 - GT Phát triển mới`, `Tháng 12 - GT Phát triển mới`, `Ktra công thức`, `Tháng tính KPI`, `Ghi chú cho KPI`, `Thanh lý năm`, Id FROM MasterVTCNTT ORDER BY `Hạng Khách hàng` LIMIT 10 OFFSET 0; ";
                "SELECT `Tuần báo cáo`, `KH nền -0 hay mới - 1`, `Qua đại lý/ KH cuối`, `Khu vực`, `Danh mục khách hàng`, `Nhóm khách hàng`, `Loại hình cơ cấu Kh`, `Kiểu Khách hàng`, `Hạng Khách hàng`, `Tên Khách hàng`, `Mã khách hàng`, `Số hợp đồng`, `Ngày ký HĐ`, `Năm NT tính cước`, `Ngày ký HĐ (nâng cấp)`, `Ngày nghiệm thu tính cước (nâng cấp)`, `Chi tiết triển khai CCDV`, `Tình trạng CCDV`, `Ngày thanh lý/giảm giá`, `Lý do thanh lý/giảm giá/tạm ngưng`, `Phân loại thanh lý/giảm giá/tạm ngưng`, `Tình trạng HĐ`, `Loại trừ KPI`, `Thời hạn HĐ`, `Loại HĐ (1 lần hay hàng tháng)`, `CID / User`, `Điểm đầu`, `Điểm cuối`, `Chi tiết dịch vụ`, `Băng thông trong nước (M)`, `Băng thông quốc tế (M)`, `Phạm vi kênh`, `Loại doanh thu`, `Loại dịch vụ`, `Tranding (Yes/No)`, `cước cài đặt (USD)`, `cước cài đặt (VNĐ)`, `Cước hàng tháng (USD) 2020`, `Cước hàng tháng (VNĐ) (trước VAT) 2020`, `Cước hàng tháng (USD) 2021`, `Cước hàng tháng (VNĐ) (trước VAT) 2021`, `Chi phí đầu vào (cước tháng)`, `Số hợp đồng đầu vào liên quan`, `Đối tác/Tỷ lệ phân chia DT`, `Tỷ lệ phân chí doanh thu`, `Tỷ lệ hoa hồng`, `Ghi chú`, `CS phụ trách`, `KD phụ trách`, `Bộ phận kí HĐ`, `Ban QL KH`, `Nền 2020 chuyển sang 2021 chốt tại thời điểm 31/12/2020`, `Tháng 1 - GT Phát triển mới`, `Tháng 2 - GT Phát triển mới`, `Tháng 3 - GT Phát triển mới`, `Tháng 4 - GT Phát triển mới`, `Tháng 5 - GT Phát triển mới`, `Tháng 6 - GT Phát triển mới`, `Tháng 7 - GT Phát triển mới`, `Tháng 8 - GT Phát triển mới`, `Tháng 9 - GT Phát triển mới`, `Tháng 10 - GT Phát triển mới`, `Tháng 11 - GT Phát triển mới`, `Tháng 12 - GT Phát triển mới`, `Ktra công thức`, `Tháng tính KPI`, `Ghi chú cho KPI`, `Thanh lý năm`, Id FROM MasterVTCNTT";

            var res = await _repository.DataConnection.QueryToListAsync<MasterVTCNTT>(sql);
            return res;
        }
    }
}