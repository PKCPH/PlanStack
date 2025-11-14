using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet
{
    public class RuleSetCreateResource : BaseUpdateCreateResource
    {
        public float Ratio { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        #region Relations
        public int StandardId { get; set; }
        public int? ComponentId { get; set; }
        public int? BuildingStructureId { get; set; }
        #endregion
    }
}
