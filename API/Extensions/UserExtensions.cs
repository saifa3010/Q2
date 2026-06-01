using System.Security.Claims;

namespace API.Extensions
{
    public static class UserExtensions
    {
        public static string GetKeycloakUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst("sub")?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("email")?.Value;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst("preferred_username")?.Value
                ?? user.Identity?.Name;
        }
    }
}