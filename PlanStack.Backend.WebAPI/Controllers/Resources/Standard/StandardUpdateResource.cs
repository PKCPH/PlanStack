using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardUpdateResource : BaseUpdateResource
    {
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }
    }
}
