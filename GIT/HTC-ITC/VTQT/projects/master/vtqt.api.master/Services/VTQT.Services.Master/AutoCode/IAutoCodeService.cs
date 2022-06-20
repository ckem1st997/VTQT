using System.Threading.Tasks;

namespace VTQT.Services.Master
{
    public partial interface IAutoCodeService
    {
        Task<string> GenerateCode(string tableName);
    }
}
