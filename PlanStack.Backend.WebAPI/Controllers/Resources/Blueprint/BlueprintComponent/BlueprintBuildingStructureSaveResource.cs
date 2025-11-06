using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure
{
    public class BlueprintComponentSaveResource : BaseResource
    {
        #region Relations
        public int? BlueprintId { get; set; }
        public int? BuildingStructureId { get; set; }
        #endregion
    }
}
