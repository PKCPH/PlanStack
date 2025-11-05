using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BSBlueprintPositionSaveResource : BaseRelationResource
    {
        public int X { get; set; }
        public int Y { get; set; }

        #region Relations
        public int BlueprintBuildingStructureId { get; set; }
        #endregion
    }
}
