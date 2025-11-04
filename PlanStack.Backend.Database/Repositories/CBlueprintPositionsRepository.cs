using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class CBlueprintPositionsRepository : BaseRelationRepository<CBlueprintPosition, CBlueprintPositionQuery>
    {
        public CBlueprintPositionsRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
