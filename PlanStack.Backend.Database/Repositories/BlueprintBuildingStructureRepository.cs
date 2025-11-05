using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;
using System.Reflection.PortableExecutable;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintBuildingStructureRepository : BaseRelationRepository<BlueprintBuildingStructure, BlueprintBuildingStructureQuery>
    {
        public BlueprintBuildingStructureRepository(DatabaseContext context) : base(context)
        {
        }

        #region GetAllByBlueprintIdAsync
        public async Task<BaseQueryResult<BlueprintBuildingStructure>> GetAllByBlueprintIdAsync(int blueprintId)
        {
            var result = new BaseQueryResult<BlueprintBuildingStructure>();
            var query = context.Set<BlueprintBuildingStructure>().AsQueryable();

            // Filtering
            query = query.Where(x => x.BlueprintId == blueprintId);

            // Querying
            var entities = await query.ToListAsync();

            result.Entities = entities;

            return result;
        }
        #endregion
    }
}
