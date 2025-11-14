using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Project
{
    public class ProjectCreateResource : BaseUpdateCreateResource
    {
        public BuildingTypeEnum BuildingType { get; set; }

        #region Relations
        //public string UserId { get; set; }
        #endregion
    }
}
