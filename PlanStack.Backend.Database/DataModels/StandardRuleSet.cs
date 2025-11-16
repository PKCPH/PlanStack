namespace PlanStack.Backend.Database.DataModels
{
    public class StandardRuleSet : BaseRelationDataModel
    {
        public string UserDefinedName { get; set; }
        public int? DefinitionValue { get; set; }
        public int? ComparisonValue { get; set; }

        #region Relations
        public int RuleSetId { get; set; }
        public virtual RuleSet RuleSet { get; set; }
        public int StandardId { get; set; }
        public virtual Standard Standard { get; set; }
        #endregion
    }
}
