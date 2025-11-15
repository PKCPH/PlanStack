namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard
{
    public class StandardRuleSetSaveResource
    {
        public string UserDefinedName { get; set; }
        public int? DefinitionValue { get; set; }
        public int? ComparisonValue { get; set; }

        public virtual int StandardId { get; set; }
        public virtual int RuleSetId { get; set; }
    }
}
