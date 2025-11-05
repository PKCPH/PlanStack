using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintBuildingStructureSaveResource
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public int StartingPositionX { get; set; }
        public int StartingPositionY { get; set; }

        public float TotalPrice { get; set; }

        #region Relations
        public int? BlueprintId { get; set; }
        public int? BuildingStructureId { get; set; }

        //public virtual List<BSBlueprintPositionSaveResource> Positions { get; set; }
        #endregion
    }
}
