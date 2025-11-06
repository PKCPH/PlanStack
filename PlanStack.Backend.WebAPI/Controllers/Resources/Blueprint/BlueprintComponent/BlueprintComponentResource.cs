using PlanStack.Backend.WebAPI.Controllers.Resources.Component;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure
{
    public class BlueprintComponentResource : BaseResource
    {
        #region Relations
        public virtual BlueprintResource Blueprint { get; set; }
        public virtual ComponentResource Component { get; set; }
        #endregion
    }
}
