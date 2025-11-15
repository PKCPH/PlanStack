namespace PlanStack.Backend.WebAPI.Controllers.Resources.Blueprint.BlueprintBuildingStructure
{
    public class BlueprintComponentSaveResource
    {
        public int StartingPositionX { get; set; }
        public int StartingPositionY { get; set; }
        public bool IsHorizontal { get; set; }

        #region Relations
        public string RoomId { get; set; }
        public int? BlueprintId { get; set; }
        public int? ComponentId { get; set; }
        #endregion
    }
}
