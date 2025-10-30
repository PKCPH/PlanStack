using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class RuleSetMappings : Profile
    {
        public RuleSetMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<RuleSetCreateResource, RuleSet>();
            CreateMap<RuleSetUpdateResource, RuleSet>();
            CreateMap<RuleSetResource, RuleSet>();

            // Mapping DataModel to Resource
            CreateMap<RuleSet, RuleSetResource>();
        }
    }
}
