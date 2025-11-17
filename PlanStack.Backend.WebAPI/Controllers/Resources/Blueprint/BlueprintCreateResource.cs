using PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintStandard;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintCreateResource : BaseUpdateCreateResource
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        #region Relations
        public string ProjectId { get; set; }

        public virtual List<BlueprintStandardCreateResource> Standards { get; set; }
        #endregion
    }
}
