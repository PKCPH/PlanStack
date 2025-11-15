using PlanStack.Backend.WebAPI.Controllers.Resources.Standard;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard
{
    public class BlueprintStandardResource
    {
        public virtual StandardResource Standard { get; set; }
        public virtual BlueprintResource Blueprint { get; set; }
    }
}
