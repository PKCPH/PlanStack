using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class RuleSetRepository : BaseRepository<RuleSet, RuleSetQuery, int>
    {
        public RuleSetRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
