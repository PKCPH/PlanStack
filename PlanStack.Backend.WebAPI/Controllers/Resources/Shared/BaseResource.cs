namespace PlanStack.Backend.WebAPI.Controllers.Resources.Shared
{
    public abstract class BaseResource<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
