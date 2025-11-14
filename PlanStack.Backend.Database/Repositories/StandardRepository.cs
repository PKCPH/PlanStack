using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class StandardRepository : BaseRepository<Standard, StandardQuery>
    {
        public StandardRepository(DatabaseContext context) : base(context)
        {
        }

        #region ApplyRelations
        protected override IQueryable<Standard> ApplyRelations(IQueryable<Standard> query)
        {
            query = query
                .Include(x => x.RuleSets);

            return base.ApplyRelations(query);
        }
        #endregion
    }
}
