namespace PlanStack.Backend.WebAPI.Controllers.Resources.User
{
    public class UserRegistrationPostResponse
    {
        public bool IsSuccessfulRegistration { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
