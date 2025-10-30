using PlanStack.Backend.WebAPI.Controllers.Resources.Shared;
using PlanStack.Shared.Enums;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.RuleSet
{
    public class RuleSetCreateResource : BaseResource
    {
        public float Ratio { get; set; }
        public RuleSetDefinitionEnum Definition { get; set; }

        #region Relations
        // TODO: Skal man altid sætte en StandardId ved oprettelse af en RuleSet?
        // Eller skal det være valgfrit?
        public int? StandardId { get; set; }
        public int? ComponentId { get; set; }
        #endregion
    }
}
