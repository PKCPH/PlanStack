using PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardResource : BaseResource<int>
    {
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }

        #region Relations
        public virtual UserResource User { get; set; }
        public virtual List<RuleSetResource> RuleSets { get; set; }
        #endregion
    }
}
