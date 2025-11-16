using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class RoomRepository : BaseRepository<Room, RoomQuery, Guid>
    {
        public RoomRepository(DatabaseContext context) : base(context)
        {
        }

        #region GetAllByBlueprintIdAsync
        public async Task<BaseQueryResult<Room>> GetAllByBlueprintIdAsync(int blueprintId, bool includeRelated = false)
        {
            var result = new BaseQueryResult<Room>();
            var query = context.Set<Room>().AsQueryable();

            // Filtering
            query = query.Where(x => x.BlueprintId == blueprintId);

            // Include relations
            if (includeRelated)
            {
                query = query.Include(x => x.Components)
                .ThenInclude(x => x.Component);
            }

            // Querying
            var entities = await query.ToListAsync();

            result.Entities = entities;

            return result;
        }
        #endregion
    }
}
