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
        #endregion
    }
}
