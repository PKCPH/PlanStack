using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.Component;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintResource : BaseResource
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        #region Relations
        //public virtual ICollection<BlueprintComponentResource> Components { get; set; }
        public virtual ICollection<BlueprintBuildingStructureResource> BuildingStructures { get; set; }
        #endregion

        #region Helpers
        public int CanvasSquareMeters => Height * Width;
        #endregion
    }
}
