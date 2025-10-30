using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class ProjectRepository : BaseRepository<Project, ProjectQuery>
    {
        public ProjectRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
