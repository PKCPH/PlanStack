using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class StandardRuleSetMappings : Profile
    {
        public StandardRuleSetMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<StandardRuleSetSaveResource, StandardRuleSet>()
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore());
            CreateMap<StandardRuleSetResource, StandardRuleSet>();

            // Mapping DataModel to Resource
            CreateMap<StandardRuleSet, StandardRuleSetResource>();
        }
    }
}
