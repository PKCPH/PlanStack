using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.DataModels
{
    public class Project : BaseDataModel<Guid>
    {
        public BuildingTypeEnum BuildingType { get; set; }
        public int SquareMeters { get; set; }

        #region Relations
        public virtual User User { get; set; }
        public virtual string UserId { get; set; }

        public virtual ICollection<Blueprint> Blueprints { get; set; }
        #endregion
    }
}
