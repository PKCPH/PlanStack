using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Room
{
    public class RoomResource : BaseResource<int>
    {
        public int SquareMeters { get; set; }
        public RoomTypeEnum RoomType { get; set; }
    }
}
