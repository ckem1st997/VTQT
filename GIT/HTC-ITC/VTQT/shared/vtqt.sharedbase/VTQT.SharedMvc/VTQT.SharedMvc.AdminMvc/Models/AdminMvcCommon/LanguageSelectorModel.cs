using System.Collections.Generic;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.AdminMvc.Models.AdminMvcCommon
{
    public class LanguageSelectorModel : BaseModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}
