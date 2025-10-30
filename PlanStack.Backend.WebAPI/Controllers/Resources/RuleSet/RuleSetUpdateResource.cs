using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet
{
    public class RuleSetUpdateResource : BaseResource
    {
        public float Ratio { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        #region Relations
        public int? ComponentId { get; set; }
        #endregion
    }
}
