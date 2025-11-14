using PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardUpdateResource : BaseUpdateCreateResource
    {
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }

        #region Relations
        public virtual List<RuleSetUpdateResource> RuleSets { get; set; }
        #endregion
    }
}
