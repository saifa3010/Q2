using Application.Common.DTOs;
using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public sealed class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
            => _sender = sender;

        /// <summary>
        /// Syncs the authenticated Keycloak user into the local Users table.
        /// Idempotent — safe to call on every login. Returns the existing user if already synced.
        /// </summary>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Sync(CancellationToken cancellationToken)
        {
            // Extract claims from the Keycloak JWT
            var keycloakUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? User.FindFirstValue("sub");

            var email    = User.FindFirstValue(ClaimTypes.Email)
                          ?? User.FindFirstValue("email")
                          ?? string.Empty;

            var username = User.FindFirstValue("preferred_username")
                          ?? User.FindFirstValue(ClaimTypes.Name)
                          ?? email;

            if (string.IsNullOrWhiteSpace(keycloakUserId))
                return Unauthorized("Missing subject claim in token.");

            var result = await _sender.Send(
                new SyncUserCommand(keycloakUserId, email, username),
                cancellationToken);

            return Ok(result);
        }
    }
}
