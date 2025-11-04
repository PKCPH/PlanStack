using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Backend.WebAPI.Controllers.Resources.User;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Project
{
    public class ProjectResource : BaseResource
    {
        public BuildingTypeEnum BuildingType { get; set; }
        public int BuildingSquareMeters { get; set; }

        #region Relations
        public virtual UserResource User { get; set; }

        public virtual ICollection<BlueprintResource> Blueprints { get; set; }
        #endregion
    }
}
