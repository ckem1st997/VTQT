using AutoMapper;
using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Infrastructure.AutoMapperProfiles;

namespace VTQT.SharedMvc.Ticket.Infrastructure.AutoMapperProfiles
{
    public class TicketProfile : Profile, IOrderedMapperProfile
    {
        public TicketProfile()
        {
            //add some generic mapping rules
            ForAllMaps(CommonProfile.AllMapsAction);

            #region TicketCategory
            CreateMap<TicketCategory, TicketCategoryModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore());

            CreateMap<TicketCategoryModel, TicketCategory>()
                .ForMember(x => x.Project, opt => opt.Ignore())
                .ForMember(x => x.Code, opt => opt.Ignore());
            #endregion

            #region StatusCategory

            CreateMap<StatusCategory, StatusCategoryModel>()
                .ForMember(x => x.Locales, opt => opt.Ignore());

            CreateMap<StatusCategoryModel, StatusCategory>()
                .ForMember(x => x.Code, opt => opt.Ignore());
            #endregion
        }

        public int Order => 1;
    }
}
