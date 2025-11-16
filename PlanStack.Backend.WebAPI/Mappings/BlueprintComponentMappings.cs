using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class BlueprintComponentMappings : Profile
    {
        public BlueprintComponentMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<BlueprintComponentSaveResource, BlueprintComponent>()
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.RoomId, opt => opt.Ignore());
            CreateMap<BlueprintComponentResource, BlueprintComponent>();

            // Mapping DataModel to Resource
            CreateMap<BlueprintComponent, BlueprintComponentResource>();
        }
    }
}
