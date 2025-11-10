

using PlanStack.Shared.Enums;

namespace PlanStack.Backend.Database.QueryModels
{
    public class ComponentQuery : BaseQueryModel
    {
        public ComponentCategoryEnum? Category { get; set; }
    }
}
