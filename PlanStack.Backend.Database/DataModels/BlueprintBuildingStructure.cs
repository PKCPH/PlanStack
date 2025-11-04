namespace PlanStack.Backend.Database.DataModels
{
    public class BlueprintBuildingStructure : BaseRelationDataModel
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public float TotalPrice { get; set; }

        #region Relations
        public virtual Blueprint Blueprint { get; set; }
        public int? BlueprintId { get; set; }

        public virtual BuildingStructure BuildingStructure { get; set; }
        public int? BuildingStructureId { get; set; }

        public virtual ICollection<BSBlueprintPosition> Positions { get; set; }
        #endregion
    }
}
