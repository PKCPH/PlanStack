using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Project
{
    public class ProjectCreateResource : BaseResource
    {
        public BuildingTypeEnum BuildingType { get; set; }
        public int SquareMeters { get; set; }

        #region Relations
        public string UserId { get; set; }
        #endregion
    }
}
