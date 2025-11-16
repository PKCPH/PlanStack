using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Room;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class RoomMappings : Profile
    {
        public RoomMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<RoomSaveResource, Room>()
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
            CreateMap<RoomResource, Room>();

            // Mapping DataModel to Resource
            CreateMap<Room, RoomResource>();
        }
    }
}
