namespace PlanStack.Backend.Database.DataModels
{
    public class BlueprintStandard : BaseDataModel
    {
        #region Relations
        public virtual Blueprint Blueprint { get; set; }
        public int? BlueprintId { get; set; }

        public virtual Standard Standard { get; set; }
        public int? StandardId { get; set; }
        #endregion
    }
}
