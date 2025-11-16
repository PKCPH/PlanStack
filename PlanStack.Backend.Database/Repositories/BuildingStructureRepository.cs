using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class BuildingStructureRepository : BaseRepository<BuildingStructure, BuildingStructureQuery, int>
    {
        public BuildingStructureRepository(DatabaseContext context) : base(context)
        {
        }

        #region ApplyFiltering
        protected override IQueryable<BuildingStructure> ApplyFiltering(IQueryable<BuildingStructure> query, BuildingStructureQuery queryModel)
        {
            if (queryModel.Category != null)
                query = query.Where(c => c.Category == queryModel.Category);

            return base.ApplyFiltering(query, queryModel);
        }
        #endregion
    }
}
