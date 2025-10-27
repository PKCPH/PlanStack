namespace PlanStack.Backend.WebAPI.Controllers.Resources.User
{
    public class UserLoginPostResponse
    {
        public bool IsAuthSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}
