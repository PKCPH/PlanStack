

namespace PlanStack.Backend.Database.QueryModels
{
    public class StandardQuery : BaseQueryModel
    {
        public string UserId { get; set; }

        public bool IsPublic { get; set; }
    }
}
