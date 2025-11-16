using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class BlueprintBuildingStructureMappings : Profile
    {
        public BlueprintBuildingStructureMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<BlueprintBuildingStructureSaveResource, BlueprintBuildingStructure>()
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());

            CreateMap<BlueprintBuildingStructureResource, BlueprintBuildingStructure>();

            // Mapping DataModel to Resource
            CreateMap<BlueprintBuildingStructure, BlueprintBuildingStructureResource>();
        }
    }
}
