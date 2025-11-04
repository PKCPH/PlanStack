namespace PlanStack.Backend.Database.DataModels
{
    public class BlueprintComponent : BaseRelationDataModel
    {
        #region Relations
        public virtual Blueprint Blueprint { get; set; }
        public int? BlueprintId { get; set; }

        public virtual Component Component { get; set; }
        public int? ComponentId { get; set; }

        public virtual ICollection<CBlueprintPosition> Positions { get; set; }
        #endregion
    }
}
