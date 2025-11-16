using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardCreateResource : BaseUpdateCreateResource
    {
        public StandardTypeEnum Type { get; set; }

        #region Relations
        public string UserId { get; set; }
        #endregion
    }
}
