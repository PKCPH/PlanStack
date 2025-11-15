using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Room
{
    public class RoomSaveResource : BaseUpdateCreateResource
    {
        public int SquareMeters { get; set; }
        public RoomTypeEnum RoomType { get; set; }

        #region Relations
        public virtual int? BlueprintId { get; set; }
        #endregion
    }
}
