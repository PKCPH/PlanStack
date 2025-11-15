using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.Room;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.Standard;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintResource : BaseResource<int>
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        #region Relations
        public virtual ICollection<BlueprintComponentResource> Components { get; set; }
        public virtual ICollection<BlueprintBuildingStructureResource> BuildingStructures { get; set; }
        public virtual ICollection<RoomResource> Rooms { get; set; }
        public virtual ICollection<StandardResource> Standards { get; set; }
        #endregion

        #region Helpers
        public int CanvasSquareMeters => Height * Width;
        #endregion
    }
}
