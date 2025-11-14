using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BlueprintRepository : BaseRepository<Blueprint, BlueprintQuery, int>
    {
        public BlueprintRepository(DatabaseContext context) : base(context)
        {
        }

        #region ApplyRelations
        protected override IQueryable<Blueprint> ApplyRelations(IQueryable<Blueprint> query)
        {
            query = query
                .Include(x => x.BuildingStructures)
                .ThenInclude(x => x.BuildingStructure)
                .Include(x => x.Components)
                .ThenInclude(x => x.Component);

            return base.ApplyRelations(query);
        }
        #endregion

        #region ApplyFiltering
        protected override IQueryable<Blueprint> ApplyFiltering(IQueryable<Blueprint> query, BlueprintQuery queryModel)
        {
            if (!string.IsNullOrEmpty(queryModel.ProjectId))
                query = query.Where(b => b.ProjectId == Guid.Parse(queryModel.ProjectId));

            return base.ApplyFiltering(query, queryModel);
        }
        #endregion
    }
}
