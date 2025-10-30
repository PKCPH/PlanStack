using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BuildingStructureRepository : BaseRepository<BuildingStructure, BuildingStructureQuery>
    {
        public BuildingStructureRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
