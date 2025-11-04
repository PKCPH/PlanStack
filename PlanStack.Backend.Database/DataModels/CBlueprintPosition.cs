namespace PlanStack.Backend.Database.DataModels
{
    public class CBlueprintPosition : BaseRelationDataModel
    {
        public int X { get; set; }
        public int Y { get; set; }

        #region Relations
        public int BlueprintComponentId { get; set; }
        public virtual BlueprintComponent BlueprintComponent { get; set; }
        #endregion
    }
}
