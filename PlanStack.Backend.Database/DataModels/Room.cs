using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class Room : BaseDataModel<Guid>
    {
        public int SquareMeters { get; set; }
        public RoomTypeEnum RoomType { get; set; }

        #region Relations
        public virtual ICollection<BlueprintComponent> Components { get; set; }
        public virtual Blueprint Blueprint { get; set; }
        public int BlueprintId { get; set; }
        #endregion
    }
}
