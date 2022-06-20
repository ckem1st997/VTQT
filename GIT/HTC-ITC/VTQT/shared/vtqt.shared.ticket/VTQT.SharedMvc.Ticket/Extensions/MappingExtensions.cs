using VTQT.Core.Domain.Ticket;
using VTQT.Core.Infrastructure.Mapper;
using VTQT.SharedMvc.Ticket.Models;

namespace VTQT.SharedMvc.Ticket
{
    public static class MappingExtensions
    {
        #region TicketCategory
        public static TicketCategoryModel ToModel(this TicketCategory entity)
        {
            return AutoMapperConfiguration.Mapper.Map<TicketCategory, TicketCategoryModel>(entity);
        }

        public static TicketCategory ToEntity(this TicketCategoryModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<TicketCategoryModel, TicketCategory>(model);
        }

        public static TicketCategory ToEntity(this TicketCategoryModel model, TicketCategory destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }
        #endregion

        #region StatusCategory

        public static StatusCategoryModel ToModel(this StatusCategory entity)
        {
            return AutoMapperConfiguration.Mapper.Map<StatusCategory, StatusCategoryModel>(entity);
        }

        public static StatusCategory ToEntity(this StatusCategoryModel model)
        {
            return AutoMapperConfiguration.Mapper.Map<StatusCategoryModel, StatusCategory>(model);
        }

        public static StatusCategory ToEntity(this StatusCategoryModel model, StatusCategory destination)
        {
            return AutoMapperConfiguration.Mapper.Map(model, destination);
        }

        #endregion
    }
}
