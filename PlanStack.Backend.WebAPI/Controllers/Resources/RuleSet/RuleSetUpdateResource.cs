using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet
{
    public class RuleSetUpdateResource : BaseUpdateCreateResource
    {
        public int DefinitionValue { get; set; }
        public int ComparisonValue { get; set; }

        #region Relations
        public int? StandardId { get; set; }
        #endregion
    }
}
