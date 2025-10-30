using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class RuleSet : BaseDataModel
    {
        public float Ratio { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        #region Relations
        public virtual Component Component { get; set; }
        public int? ComponentId { get; set; }

        public virtual ICollection<StandardRuleSet> StandardRuleSets { get; set; }
        #endregion
    }
}
