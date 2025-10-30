using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Component;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class ComponentMappings : Profile
    {
        public ComponentMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<ComponentCreateResource, Component>();
            CreateMap<ComponentUpdateResource, Component>();
            CreateMap<ComponentResource, Component>();

            // Mapping DataModel to Resource
            CreateMap<Component, ComponentResource>();
        }
    }
}
