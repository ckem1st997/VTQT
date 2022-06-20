using System.Collections.Generic;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class FTTH2022AddOrUpdateModel
    {
        public IEnumerable<FTTH2022Model> models { get; set; }
        public IEnumerable<FTTH2022Model> modelsAdd { get; set; }
    }
}