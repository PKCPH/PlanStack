using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class BuildingStructure : BaseDataModel<int>
    {
        public BuildingStructureCategoryEnum Category { get; set; }
        public string Material { get; set; }
        public float Price { get; set; }

        #region
        public virtual ICollection<BlueprintBuildingStructure> BlueprintBuildingStructures { get; set; }
        #endregion
    }
}
