namespace PlanStack.Backend.WebAPI.Controllers.Resources.Shared
{
    public class BaseQueryResultResource<T>
    {
        public IEnumerable<T> Entities { get; set; }
        public int Count { get; set; }
    }
}
