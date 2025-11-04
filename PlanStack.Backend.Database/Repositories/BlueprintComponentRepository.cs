using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintComponentRepository : BaseRelationRepository<BlueprintComponent, BlueprintComponentQuery>
    {
        public BlueprintComponentRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
