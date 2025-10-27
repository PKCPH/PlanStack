namespace PlanStack.Backend.Database.DataModels
{
    public class Blueprint : BaseDataModel
    {
        public int MaxOccupancy { get; set; }
        public int Floor { get; set; }
        public int SquareMeters { get; set; }

        #region Relations
        #endregion
    }
}
