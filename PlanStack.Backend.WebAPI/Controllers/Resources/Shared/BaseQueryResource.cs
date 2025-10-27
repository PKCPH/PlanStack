namespace PlanStack.Backend.WebAPI.Controllers.Resources.Shared
{
    public class BaseQueryResource
    {
        public string SortBy { get; set; }

        public bool IsSortAscending { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; } = -1;
    }
}
