using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class StandardRuleSetRepository : BaseRelationRepository<StandardRuleSet, StandardRuleSetQuery>
    {
        public StandardRuleSetRepository(DatabaseContext context) : base(context)
        {
        }

        #region GetAllByStandardIdAsync
        public async Task<BaseQueryResult<StandardRuleSet>> GetAllByStandardIdAsync(int standardId)
        {
            var result = new BaseQueryResult<StandardRuleSet>();
            var query = context.Set<StandardRuleSet>().AsQueryable();

            // Filtering
            query = query.Where(x => x.StandardId == standardId);

            // Including Relations
            query = query.Include(x => x.RuleSet);

            // Querying
            var entities = await query.ToListAsync();

            result.Entities = entities;

            return result;
        }
        #endregion
    }
}
