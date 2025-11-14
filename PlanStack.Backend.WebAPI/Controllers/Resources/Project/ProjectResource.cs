using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Project
{
    public class ProjectResource : BaseResource<Guid>
    {
        public BuildingTypeEnum BuildingType { get; set; }
        public int SquareMeters { get; set; }

        #region Relations
        //public virtual UserResource User { get; set; }

        //public virtual ICollection<BlueprintResource> Blueprints { get; set; }
        #endregion
    }
}
