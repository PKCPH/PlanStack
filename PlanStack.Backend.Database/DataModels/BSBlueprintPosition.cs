namespace PlanStack.Backend.Database.DataModels
{
    public class BSBlueprintPosition : BaseRelationDataModel
    {
        public int X { get; set; }
        public int Y { get; set; }

        #region Relations
        public int BlueprintBuildingStructureId { get; set; }
        public virtual BlueprintBuildingStructure BlueprintBuildingStructure { get; set; }
        #endregion
    }
}
