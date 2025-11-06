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
            CreateMap<BlueprintSaveResource, Blueprint>();

            // Mapping DataModel to Resource
            CreateMap<Blueprint, BlueprintResource>();
        }
    }
}
