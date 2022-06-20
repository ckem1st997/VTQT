using System.Collections.Generic;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class MasterVTCNTTAddOrUpdateModel
    {
        public IEnumerable<MasterVTCNTTModel> models { get; set; }
        public IEnumerable<MasterVTCNTTModel> modelsAdd { get; set; }
    }
}