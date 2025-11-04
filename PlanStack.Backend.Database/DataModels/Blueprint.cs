namespace PlanStack.Backend.Database.DataModels
{
    public class Blueprint : BaseDataModel
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int SquareMeters { get; set; }

        #region Relations
        public virtual List<BlueprintComponent> Components { get; set; }
        public virtual List<BlueprintBuildingStructure> BuildingStructures { get; set; }
        #endregion
    }
}
