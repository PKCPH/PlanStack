using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<UserRegistrationPostRequest, User>();
            CreateMap<UserResource, User>();

            // Mapping DataModel to Resource
            CreateMap<User, UserResource>();
        }
    }
}
