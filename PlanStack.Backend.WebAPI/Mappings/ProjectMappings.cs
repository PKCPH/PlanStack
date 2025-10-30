using AutoMapper;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Project;

namespace PlanStack.Backend.WebAPI.Mappings
{
    public class ProjectMappings : Profile
    {
        public ProjectMappings()
        {
            // Mapping Resource to DataModel
            CreateMap<ProjectCreateResource, Project>();
            CreateMap<ProjectUpdateResource, Project>();
            CreateMap<ProjectResource, Project>();

            // Mapping DataModel to Resource
            CreateMap<Project, ProjectResource>();
        }
    }
}
