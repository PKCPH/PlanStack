using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintStandardRepository : BaseRelationRepository<BlueprintStandard, BlueprintStandardQuery>
    {
        public BlueprintStandardRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
