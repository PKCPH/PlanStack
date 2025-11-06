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
            CreateMap<BlueprintComponentSaveResource, BlueprintComponent>();
            CreateMap<BlueprintComponentResource, BlueprintComponent>();

            // Mapping DataModel to Resource
            CreateMap<BlueprintComponent, BlueprintComponentResource>();
        }
    }
}
