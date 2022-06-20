using System.Threading.Tasks;
using VTQT.Core.Domain.Master;
using VTQT.Core.Infrastructure;
using VTQT.Data;
using System;
using System.Linq;

namespace VTQT.Services.Master
{
    public partial class AutoCodeService : IAutoCodeService
    {
        #region Fields
        private readonly IRepository<AutoCode> _autoCodeRepository;
        private const int MAX_NUMBER = 999999;
        #endregion

        #region Ctor
        public AutoCodeService()
        {
            _autoCodeRepository = EngineContext.Current.Resolve<IRepository<AutoCode>>(DataConnectionHelper.ConnectionStringNames.Master);
        }
        #endregion

        #region Methods
        public async Task<string> GenerateCode(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var entity = _autoCodeRepository.Table.FirstOrDefault(x => x.TableName == tableName);

            if (entity == null)
            {
                var autoCode = new AutoCode
                {
                    TableName = tableName,
                    Prefix = "",
                    NumberPart = 1,
                    Suffix = "",
                    NumberPartMinLength = 6
                };

                var save = await _autoCodeRepository.InsertAsync(autoCode);
                var code = autoCode.Prefix + "00000" + autoCode.NumberPart + autoCode.Suffix;
                return code;
            }
            else
            {
                var lastestNumberPart = entity.NumberPart;
                var newNumberPart = ++lastestNumberPart;
                if (newNumberPart <= MAX_NUMBER)
                {
                    entity.NumberPart = newNumberPart;
                    var update = await _autoCodeRepository.UpdateAsync(entity);
                    var numberOfLastNumberPart = GetNumberOf(newNumberPart);
                    if (numberOfLastNumberPart == entity.NumberPartMinLength)
                    {                        
                        return entity.Prefix + entity.NumberPart + entity.Suffix;
                    }
                    else
                    {
                        var numberOfZero = entity.NumberPartMinLength - numberOfLastNumberPart;
                        var stringNumberPart = entity.NumberPart.ToString();
                        while(numberOfZero > 0)
                        {
                            stringNumberPart = "0" + stringNumberPart;
                            numberOfZero--;
                        }
                        return entity.Prefix + stringNumberPart + entity.Suffix;
                    }
                }
                else
                {
                    return null;
                }                
            }
        }
        #endregion

        #region Utilities
        private int GetNumberOf(int numberPart)
        {
            int number = 1;
            while((numberPart / 10) > 0)
            {
                number++;
                numberPart /= 10;
            }
            return number;
        }
        #endregion
    }
}
