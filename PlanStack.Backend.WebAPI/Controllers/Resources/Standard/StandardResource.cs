using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardResource : BaseResource
    {
        public StandardTypeEnum Type { get; set; }
        public bool IsPublic { get; set; }

        #region Relations
        public virtual UserResource User { get; set; }
        #endregion
    }
}
