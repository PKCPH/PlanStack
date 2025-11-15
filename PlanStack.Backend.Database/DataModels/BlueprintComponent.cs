namespace PlanStack.Backend.Database.DataModels
{
    public class BlueprintComponent : BaseRelationDataModel
    {
        public int StartingPositionX { get; set; }
        public int StartingPositionY { get; set; }
        public bool IsHorizontal { get; set; }

        #region Relations
        public virtual Room Room { get; set; }
        public virtual Guid? RoomId { get; set; }

        public virtual Blueprint Blueprint { get; set; }
        public virtual int BlueprintId { get; set; }

        public virtual Component Component { get; set; }
        public virtual int ComponentId { get; set; }
        #endregion
    }
}
