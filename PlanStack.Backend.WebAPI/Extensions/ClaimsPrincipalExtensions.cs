using System.Security.Claims;

namespace PlanStack.Backend.WebAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var value = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(value, out var guid))
                return null;

            return guid;
        }
    }
}
