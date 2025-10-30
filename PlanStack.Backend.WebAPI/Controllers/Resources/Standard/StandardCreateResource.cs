using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Standard
{
    public class StandardCreateResource : BaseResource
    {
        public StandardTypeEnum Type { get; set; }

        #region Relations
        public string UserId { get; set; }
        #endregion
    }
}
