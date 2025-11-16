using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure
{
    public class BuildingStructureQueryResource : BaseQueryResource
    {
        public int? Category { get; set; }
    }
}
