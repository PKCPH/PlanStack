using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintRepository : BaseRepository<Blueprint, BlueprintQuery>
    {
        public BlueprintRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
