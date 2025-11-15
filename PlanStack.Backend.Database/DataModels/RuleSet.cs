using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class RuleSet : BaseDataModel<int>
    {
        public int DefinitionValue { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        public int ComparisonValue { get; set; }
        public RuleSetComparisonEnum Comparison { get; set; }

        public RuleSetObjectTypeEnum? ObjectTypeDefinition { get; set; }
        public RuleSetObjectTypeEnum? ObjectTypeComparison { get; set; }

        #region Relations
        public virtual Standard Standard { get; set; }
        public int? StandardId { get; set; }
        #endregion
    }
}
