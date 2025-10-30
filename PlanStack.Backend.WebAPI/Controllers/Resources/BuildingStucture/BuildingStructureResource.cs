using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure
{
    public class BuildingStructureResource : BaseResource
    {
        public string Material { get; set; }
        public int Price { get; set; }
        public int SquareMeters { get; set; }
    }
}
