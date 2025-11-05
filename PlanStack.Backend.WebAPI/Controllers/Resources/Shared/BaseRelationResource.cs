namespace PlanStack.Backend.WebAPI.Controllers.Resources.Shared
{
    public abstract class BaseRelationResource
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
