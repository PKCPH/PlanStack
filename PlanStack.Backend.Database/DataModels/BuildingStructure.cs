using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class BuildingStructure : BaseDataModel
    {
        public BuildingStructureCategoryEnum Category { get; set; }
        public string Material { get; set; }
        public int Price { get; set; }
        public int SquareMeters { get; set; }
    }
}
