using PlanStack.Backend.WebAPI.Controllers.Resources.BuildingStructure;
using PlanStack.Backend.WebAPI.Controllers.Resources.Component;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet
{
    public class RuleSetResource : BaseResource
    {
        public float Ratio { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        #region Relations
        public virtual ComponentResource Component { get; set; }
        public virtual BuildingStructureResource BuildingStructure { get; set; }
        #endregion
    }
}
