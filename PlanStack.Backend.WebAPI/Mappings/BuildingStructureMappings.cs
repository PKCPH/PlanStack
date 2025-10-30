using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class BuildingStructureMappings : Profile
    {
        public BuildingStructureMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<BuildingStructureCreateResource, BuildingStructure>();
            CreateMap<BuildingStructureUpdateResource, BuildingStructure>();
            CreateMap<BuildingStructureResource, BuildingStructure>();

            // Mapping DataModel to Resource
            CreateMap<BuildingStructure, BuildingStructureResource>();
        }
    }
}
