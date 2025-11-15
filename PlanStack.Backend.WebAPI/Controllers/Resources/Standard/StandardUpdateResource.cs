using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardUpdateResource : BaseUpdateCreateResource
    {
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }

        #region Relations
        public virtual List<StandardRuleSetSaveResource> RuleSets { get; set; }
        #endregion
    }
}
