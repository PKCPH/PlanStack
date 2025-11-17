using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintComponentRepository : BaseRelationRepository<BlueprintComponent, BlueprintComponentQuery>
    {
        public BlueprintComponentRepository(DatabaseContext context) : base(context)
        {
        }

        #region GetAllByBlueprintIdAsync
        public async Task<BaseQueryResult<BlueprintComponent>> GetAllByBlueprintIdAsync(int blueprintId, bool includeRelated = false)
        {
            var result = new BaseQueryResult<BlueprintComponent>();
            var query = context.Set<BlueprintComponent>().AsQueryable();

            // Filtering
            query = query.Where(x => x.BlueprintId == blueprintId);

            // Include relations
            if (includeRelated)
            {
                query = query.Include(x => x.Component);
            }

            // Querying
            var entities = await query.ToListAsync();

            result.Entities = entities;

            return result;
        }
        #endregion
    }
}
