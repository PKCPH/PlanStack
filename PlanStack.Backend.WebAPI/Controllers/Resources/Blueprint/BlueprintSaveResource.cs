using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint
{
    public class BlueprintSaveResource
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        #region Relations
        //public virtual List<BlueprintComponent> Components { get; set; }
        public virtual List<BlueprintBuildingStructureSaveResource> BuildingStructures { get; set; }
        #endregion
    }
}
