using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Component
{
    public class ComponentResource : BaseResource<int>
    {
        public ComponentCategoryEnum Category { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Model { get; set; }
        public int Price { get; set; }
        public int SquareMeters { get; set; }
        public string ImgPath { get; set; }
    }
}
