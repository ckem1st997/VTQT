using System.Collections.Generic;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class MasterFileKH2022AddOrUpdateModel
    {
        public IEnumerable<MasterFileKH2022Model> models { get; set; }
        public IEnumerable<MasterFileKH2022Model> modelsAdd { get; set; }
    }
}