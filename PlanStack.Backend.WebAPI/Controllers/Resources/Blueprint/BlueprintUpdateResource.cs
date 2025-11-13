using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintUpdateResource : BaseUpdateCreateResource
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int SquareMeters { get; set; }
    }
}
