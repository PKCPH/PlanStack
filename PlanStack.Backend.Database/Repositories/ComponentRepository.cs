using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class ComponentRepository : BaseRepository<Component, ComponentQuery>
    {
        public ComponentRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
