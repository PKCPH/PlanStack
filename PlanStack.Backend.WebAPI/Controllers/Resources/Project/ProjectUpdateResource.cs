using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Project
{
    public class ProjectUpdateResource : BaseUpdateCreateResource
    {
        public BuildingTypeEnum BuildingType { get; set; }
        public int SquareMeters { get; set; }
    }
}
