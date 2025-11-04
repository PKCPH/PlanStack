

namespace PlanStack.Backend.Database.QueryModels
{
    public class BaseQueryResult<T>
    {
        public List<T> Entities { get; set; }
        public int Count { get; set; }
    }
}
