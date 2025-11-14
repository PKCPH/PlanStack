using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class Standard : BaseDataModel<int>
    {
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }

        #region Relations
        public virtual User User { get; set; }
        public virtual string UserId { get; set; }

        public virtual ICollection<RuleSet> RuleSets { get; set; }
        #endregion
    }
}
