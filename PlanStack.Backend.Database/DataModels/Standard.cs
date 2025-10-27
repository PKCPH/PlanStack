using PlanStack.Backend.Database.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class Standard : BaseDataModel
    {
        public string Description { get; set; }
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }

        #region Relations
        public virtual User User { get; set; }
        public virtual string UserId { get; set; }

        public virtual ICollection<StandardRuleSet> StandardRuleSets { get; set; }
        #endregion
    }
}
