using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class BlueprintMappings : Profile
    {
        public BlueprintMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<BlueprintCreateResource, Blueprint>();
            CreateMap<BlueprintUpdateResource, Blueprint>();
            CreateMap<BlueprintResource, Blueprint>();

            CreateMap<BlueprintSaveResource, Blueprint>()
                .ForMember(d => d.Rooms, opt => opt.Ignore())
                .ForMember(d => d.Components, opt => opt.Ignore())
                .ForMember(d => d.BuildingStructures, opt => opt.Ignore())
                .ForMember(d => d.Standards, opt => opt.Ignore());

            // Mapping DataModel to Resource
            CreateMap<Blueprint, BlueprintResource>();
        }
    }
}
