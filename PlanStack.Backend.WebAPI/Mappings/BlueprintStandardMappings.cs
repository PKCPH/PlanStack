using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class BlueprintStandardMappings : Profile
    {
        public BlueprintStandardMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<BlueprintStandardSaveResource, BlueprintStandard>()
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
            CreateMap<BlueprintStandardResource, BlueprintStandard>();

            // Mapping DataModel to Resource
            CreateMap<BlueprintStandard, BlueprintStandardResource>();
        }
    }
}
