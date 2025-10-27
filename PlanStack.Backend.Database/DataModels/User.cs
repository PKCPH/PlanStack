using Microsoft.AspNetCore.Identity;

namespace PlanStack.Backend.Database.DataModels
{
    public class User : IdentityUser
    {
        #region Relations
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Standard> Standards { get; set; }
        #endregion
    }
}
