using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Component
{
    public class ComponentUpdateResource
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ComponentCategoryEnum Category { get; set; }
        public string Model { get; set; }
        public int Price { get; set; }
        public int SquareMeters { get; set; }
    }
}
