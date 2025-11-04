using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class Component : BaseDataModel
    {
        public ComponentCategoryEnum Category { get; set; }
        public string Model { get; set; }
        public float Price { get; set; }
        public int SquareMeters { get; set; }
        public string ImgPath { get; set; }
    }
}
