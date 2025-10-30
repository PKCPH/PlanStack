using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Standard;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class StandardMappings : Profile
    {
        public StandardMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<StandardCreateResource, Standard>();
            CreateMap<StandardUpdateResource, Standard>();
            CreateMap<StandardResource, Standard>();

            // Mapping DataModel to Resource
            CreateMap<Standard, StandardResource>();
        }
    }
}
