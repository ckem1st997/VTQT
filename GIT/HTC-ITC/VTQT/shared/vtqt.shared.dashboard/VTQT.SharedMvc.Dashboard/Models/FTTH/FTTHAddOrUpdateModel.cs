using System.Collections.Generic;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class FTTHAddOrUpdateModel
    {
        public IEnumerable<FTTHModel> models { get; set; }
        public IEnumerable<FTTHModel> modelsAdd { get; set; }
    }
}