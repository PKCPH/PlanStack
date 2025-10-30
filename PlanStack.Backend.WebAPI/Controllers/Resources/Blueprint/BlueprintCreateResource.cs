using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintCreateResource : BaseResource
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int SquareMeters { get; set; }

        #region Relations
        #endregion
    }
}
