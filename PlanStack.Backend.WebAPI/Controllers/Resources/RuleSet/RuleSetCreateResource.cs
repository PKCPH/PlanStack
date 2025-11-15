using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet
{
    public class RuleSetCreateResource : BaseUpdateCreateResource
    {
        public int DefinitionValue { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        public int ComparisonValue { get; set; }
        public RuleSetComparisonEnum Comparison { get; set; }

        public RuleSetObjectTypeEnum? ObjectTypeDefinition { get; set; }
        public RuleSetObjectTypeEnum? ObjectTypeComparison { get; set; }

        #region Relations
        public int? StandardId { get; set; }
        #endregion
    }
}
