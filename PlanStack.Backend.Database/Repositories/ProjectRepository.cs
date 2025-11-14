using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class ProjectRepository : BaseRepository<Project, ProjectQuery, Guid>
    {
        public ProjectRepository(DatabaseContext context) : base(context)
        {
        }

        #region ApplyRelations
        protected override IQueryable<Project> ApplyRelations(IQueryable<Project> query)
        {
            query = query
                .Include(x => x.Blueprints);

            return base.ApplyRelations(query);
        }
        #endregion

        #region ApplyFiltering
        protected override IQueryable<Project> ApplyFiltering(IQueryable<Project> query, ProjectQuery queryModel)
        {
            if (!string.IsNullOrEmpty(queryModel.UserId))
                query = query.Where(p => p.UserId == queryModel.UserId);

            return base.ApplyFiltering(query, queryModel);
        }
        #endregion
    }
}
