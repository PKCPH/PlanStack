using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.User
{
    public class ComponentUpdateResource : BaseResource
    {
        public string Model { get; set; }
        public int Price { get; set; }
        public int SquareMeters { get; set; }
    }
}
