namespace PlanStack.Backend.Database.DataModels
{
    public class Blueprint : BaseDataModel<int>
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public bool IsValidated { get; set; }

        #region Relations
        public virtual Project Project { get; set; }
        public Guid ProjectId { get; set; }
        public virtual List<Room> Rooms { get; set; }
        public virtual List<BlueprintComponent> Components { get; set; }
        public virtual List<BlueprintBuildingStructure> BuildingStructures { get; set; }
        public virtual List<BlueprintStandard> Standards { get; set; }
        #endregion
    }
}
