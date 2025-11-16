using PlanStack.Backend.WebAPI.Controllers.Resources.Project;
using PlanStack.Backend.WebAPI.Controllers.Resources.Standard;

namespace PlanStack.Backend.WebAPI.Controllers.Resources.User
{
    public class UserResource
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        #region Relations
        public virtual ICollection<StandardResource> Standards { get; set; }
        public virtual ICollection<ProjectResource> Projects { get; set; }
        #endregion
    }
}
