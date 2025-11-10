using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Component
{
    public class ComponentQueryResource : BaseQueryResource
    {
        public int? Category { get; set; }
    }
}
