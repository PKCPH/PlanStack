using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintBuildingStructureRepository : BaseRepository<BlueprintBuildingStructure, BlueprintBuildingStructureQuery>
    {
        public BlueprintBuildingStructureRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
