using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;
using PlanStack.Backend.WebAPI.Controllers.Resources.Room;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintSaveResource : BaseUpdateCreateResource
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        #region Relations
        public virtual List<RoomSaveResource> Rooms { get; set; }
        public virtual List<BlueprintComponentSaveResource> Components { get; set; }
        public virtual List<BlueprintBuildingStructureSaveResource> BuildingStructures { get; set; }
        public virtual List<BlueprintStandardSaveResource> Standards { get; set; }
        #endregion
    }
}
