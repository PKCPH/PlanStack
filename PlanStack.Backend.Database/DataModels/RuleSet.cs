using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class RuleSet : BaseDataModel<int>
    {
        public RuleSetDefinitionEnum Definition { get; set; }
        public RuleSetComparisonEnum? Comparison { get; set; }

        public RuleSetObjectTypeEnum ObjectTypeDefinition { get; set; }
        public RuleSetObjectTypeEnum? ObjectTypeComparison { get; set; }

        #region Relations
        public virtual ICollection<StandardRuleSet> Standards { get; set; }
        #endregion
    }
}
