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
        public int SquareMeters { get; set; }

        #region Relations
        public virtual ICollection<ComponentResource> Components { get; set; }
        public virtual ICollection<BuildingStructureResource> BuildingStructures { get; set; }
        #endregion
    }
}
