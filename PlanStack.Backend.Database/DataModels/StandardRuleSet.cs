namespace PlanStack.Backend.Database.DataModels
{
    public class StandardRuleSet : BaseRelationDataModel
    {
        #region Relations
        public virtual Standard Standard { get; set; }
        public int? StandardId { get; set; }

        public virtual RuleSet RuleSet { get; set; }
        public int? RuleSetId { get; set; }
        #endregion
    }
}
