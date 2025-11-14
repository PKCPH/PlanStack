using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class RuleSet : BaseDataModel<int>
    {
        public float Ratio { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        #region Relations
        public virtual Component Component { get; set; }
        public int? ComponentId { get; set; }

        public virtual BuildingStructure BuildingStructure { get; set; }
        public int? BuildingStructureId { get; set; }

        public virtual Standard Standard { get; set; }
        public int StandardId { get; set; }
        #endregion
    }
}
