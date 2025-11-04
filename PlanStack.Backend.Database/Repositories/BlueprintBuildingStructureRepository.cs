using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintBuildingStructureRepository : BaseRelationRepository<BlueprintBuildingStructure, BlueprintBuildingStructureQuery>
    {
        public BlueprintBuildingStructureRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
