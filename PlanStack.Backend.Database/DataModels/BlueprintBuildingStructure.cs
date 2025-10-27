namespace PlanStack.Backend.Database.DataModels
{
    public class BlueprintBuildingStructure : BaseRelationModel
    {
        public int positionX { get; set; }
        public int positionY { get; set; }

        #region Relations
        public virtual Blueprint Blueprint { get; set; }
        public int? BlueprintId { get; set; }

        public virtual BuildingStructure BuildingStructure { get; set; }
        public int? BuildingStructureId { get; set; }
        #endregion
    }
}
