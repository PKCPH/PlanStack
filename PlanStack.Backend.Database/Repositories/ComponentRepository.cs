using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class ComponentRepository : BaseRepository<Component, ComponentQuery, int>
    {
        public ComponentRepository(DatabaseContext context) : base(context)
        {
        }

        #region ApplyFiltering
        protected override IQueryable<Component> ApplyFiltering(IQueryable<Component> query, ComponentQuery queryModel)
        {
            if (queryModel.Category != null)
                query = query.Where(c => c.Category == queryModel.Category);

            return base.ApplyFiltering(query, queryModel);
        }
        #endregion
    }
}
