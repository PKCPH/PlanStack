

using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.QueryModels
{
    public class BuildingStructureQuery : BaseQueryModel
    {
        public BuildingStructureCategoryEnum? Category { get; set; }
    }
}
