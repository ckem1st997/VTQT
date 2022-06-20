using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core.Infrastructure;
using VTQT.Data;

namespace VTQT.Services.File
{
    public class FilesService : IFilesService
    {
        #region Fields

        private readonly IRepository<Core.Domain.File.Files> _fileRepository;

        #endregion

        #region Ctor
        public FilesService()
        {
            _fileRepository = EngineContext.Current.Resolve<IRepository<Core.Domain.File.Files>>(DataConnectionHelper.ConnectionStringNames.File);
        }

        #endregion

        #region Method

        public virtual async Task<long> InsertRangeAsync(IEnumerable<Core.Domain.File.Files> entities)
        {
            try
            {
                if (entities is null)
                    throw new ArgumentNullException(nameof(entities));
                var result = await _fileRepository.InsertAsync(entities);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<Core.Domain.File.Files> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await _fileRepository.GetByIdAsync(id);

            return result;
        }
        #endregion
    }
}
