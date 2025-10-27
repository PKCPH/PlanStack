namespace PlanStack.Backend.WebAPI.Controllers.Resources.User
{
    public class UserUpdateRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
