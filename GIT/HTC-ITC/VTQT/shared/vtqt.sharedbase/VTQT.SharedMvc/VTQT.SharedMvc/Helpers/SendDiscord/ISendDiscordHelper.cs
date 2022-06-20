using System.Collections.Generic;
using System.Threading.Tasks;
using VTQT.SharedMvc.Master.Models;

namespace VTQT.SharedMvc.Helpers
{
    public interface ISendDiscordHelper
    {
        Task<object> SendMessage(string content);
    }
}
