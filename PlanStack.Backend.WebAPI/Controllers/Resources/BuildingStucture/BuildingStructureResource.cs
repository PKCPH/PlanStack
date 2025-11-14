using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure
{
    public class BuildingStructureResource : BaseResource<int>
    {
        public BuildingStructureCategoryEnum Category { get; set; }
        public string Material { get; set; }
        public int Price { get; set; }
    }
}
