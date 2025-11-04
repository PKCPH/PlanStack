using AutoMapper;
using PlanStack.Backend.Database.QueryModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class BaseMappings : Profile
    {
        public BaseMappings()
        {
            // Mapping DataModel to Resource
            CreateMap(typeof(BaseQueryResult<>), typeof(BaseQueryResultResource<>));
        }
    }
}
