using PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard
{
    public class StandardRuleSetResource
    {
        public string UserDefinedName { get; set; }
        public int DefinitionValue { get; set; }
        public int ComparisonValue { get; set; }
        public virtual RuleSetResource RuleSet { get; set; }
    }
}
