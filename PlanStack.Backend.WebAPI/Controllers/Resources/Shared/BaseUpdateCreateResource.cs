namespace PlanStack.Backend.WebAPI.Controllers.Resources.Shared
{
    public abstract class BaseUpdateCreateResource
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
