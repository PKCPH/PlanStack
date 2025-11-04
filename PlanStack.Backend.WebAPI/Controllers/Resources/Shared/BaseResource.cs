namespace PlanStack.Backend.WebAPI.Controllers.Resources.Shared
{
    public abstract class BaseResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
