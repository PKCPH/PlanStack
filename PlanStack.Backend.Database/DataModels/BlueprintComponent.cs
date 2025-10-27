namespace PlanStack.Backend.Database.DataModels
{
    public class BlueprintComponent : BaseRelationModel
    {
        public int positionX { get; set; }
        public int positionY { get; set; }

        #region Relations
        public virtual Blueprint Blueprint { get; set; }
        public int? BlueprintId { get; set; }

        public virtual Component Component { get; set; }
        public int? ComponentId { get; set; }
        #endregion
    }
}
