using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class StandardRuleSetRepository : BaseRelationRepository<StandardRuleSet, StandardRuleSetQuery>
    {
        public StandardRuleSetRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
