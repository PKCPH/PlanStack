using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure
{
    public class BlueprintComponentSaveResource
    {
        public int StartingPositionX { get; set; }
        public int StartingPositionY { get; set; }

        #region Relations
        public int? BlueprintId { get; set; }
        public int? ComponentId { get; set; }
        #endregion
    }
}
