using PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure
{
    public class BlueprintBuildingStructureResource
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public int StartingPositionX { get; set; }
        public int StartingPositionY { get; set; }

        public float TotalPrice { get; set; }

        #region
        //public virtual BlueprintResource Blueprint { get; set; }
        public virtual BuildingStructureResource BuildingStructure { get; set; }
        #endregion
    }
}
