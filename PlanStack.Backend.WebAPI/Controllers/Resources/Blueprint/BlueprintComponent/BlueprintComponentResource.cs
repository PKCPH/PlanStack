using PlanStack.Backend.WebAPI.Controllers.Resources.Component;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure
{
    public class BlueprintComponentResource
    {
        public int StartingPositionX { get; set; }
        public int StartingPositionY { get; set; }
        public bool IsHorizontal { get; set; }

        #region Relations
        public virtual ComponentResource Component { get; set; }
        #endregion
    }
}
