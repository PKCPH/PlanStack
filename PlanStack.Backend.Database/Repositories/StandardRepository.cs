using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public class StandardRepository : BaseRepository<Standard, StandardQuery>
    {
        public StandardRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
