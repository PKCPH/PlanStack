using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class StandardRepository : BaseRepository<Standard, StandardQuery, int>
    {
        public StandardRepository(DatabaseContext context) : base(context)
        {
        }

        #region ApplyRelations
        protected override IQueryable<Standard> ApplyRelations(IQueryable<Standard> query)
        {
            query = query
                .Include(x => x.RuleSets)
                .ThenInclude(x => x.RuleSet);

            return base.ApplyRelations(query);
        }
        #endregion
    }
}
