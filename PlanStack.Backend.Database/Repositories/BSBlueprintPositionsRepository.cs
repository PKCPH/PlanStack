using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BSBlueprintPositionsRepository : BaseRelationRepository<BSBlueprintPosition, BSBlueprintPositionQuery>
    {
        public BSBlueprintPositionsRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
