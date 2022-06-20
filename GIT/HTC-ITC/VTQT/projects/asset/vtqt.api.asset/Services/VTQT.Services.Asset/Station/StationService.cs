using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.Core.Domain.Asset;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System.Linq;
using VTQT.Core.Domain.Qlsc;

namespace VTQT.Services.Asset
{
    public partial class StationService : IStationService
    {
        #region Fields
        private readonly IRepository<Station> _stationRepository;
        private readonly IIntLowercaseRepository<danh_sach_tram> _stationQlscRepository;
        #endregion

        #region Ctor
        public StationService()
        {
            _stationRepository = EngineContext.Current.Resolve<IRepository<Station>>(DataConnectionHelper.ConnectionStringNames.Asset);
            _stationQlscRepository = EngineContext.Current.Resolve<IIntLowercaseRepository<danh_sach_tram>>(DataConnectionHelper.ConnectionStringNames.Qlsc);
        }
        #endregion

        #region Methods
        public async Task<Station> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entity = await _stationRepository.GetByIdAsync(id);

            return entity;
        }

        public void UpdateAll()
        {
            var qlsc = from p in _stationQlscRepository.Table select p;            

            if (qlsc?.ToList().Count > 0)
            {
                qlsc.ToList().ForEach(async x =>
                {
                    var station = new Station
                    {
                        Code = x.ma_tram,
                        Name = x.ten_tram,
                        CodeVnm = x.ma_tram_vnm,
                        Area = x.khu_vuc,
                        Province = x.ten_tinh,
                        Level = x.phan_cap,
                        Address = x.dia_chi,
                        Longitude = x.kinh_do,
                        Latitude = x.vi_do,
                        Note = x.note,
                        CategoryId = x.loai_tram
                    };

                    var entity = _stationRepository.Table.FirstOrDefault(s => s.Code == x.ma_tram);
                    if (entity != null)
                    {
                        await _stationRepository.UpdateAsync(station);
                    }
                    else
                    {
                        await _stationRepository.InsertAsync(station);
                    }
                });
            }
        }

        public async Task<int> UpdateAsync(Station entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _stationRepository.UpdateAsync(entity);

            return result;
        }

        public async Task<int> InsertAsync(Station entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _stationRepository.InsertAsync(entity);

            return result;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return await _stationRepository.Table
                .AnyAsync(x => x.Code != null &&
                          x.Code.Equals(code));
        }

        public async Task<Station> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var result = await _stationRepository.Table.FirstOrDefaultAsync(x => x.Code == code);

            return result;
        }
        #endregion

        #region List
        public IList<Station> GetAll()
        {
            var query = from p in _stationRepository.Table select p;

            return query.ToList();
        }
        #endregion
    }
}
