namespace PlanStack.Backend.Database.DataModels
{
    public class Component : BaseDataModel
    {
        public string Model { get; set; }
        public int Price { get; set; }
        public int SquareMeters { get; set; }
    }
}
