using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarApi.Infrastructure.Business;
using StellarApi.RestApi.Auth;
using System.Security.Claims;

namespace StellarApi.RestApi.Controllers
{
    /// <summary>
    /// Represents the API controller for managing tokens.
    /// </summary>
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/tokens/")]
    public class TokenController : ControllerBase
    {
        /// <summary>
        /// Represents the user context for the application.
        /// </summary>
        private readonly IUserService _service;

        /// <summary>
        /// Represents the token service for the application.
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Creates a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        /// <param name="userService">The user service for the application.</param>
        /// <param name="tokenService">The token service for the application.</param>
        public TokenController(IUserService userService, ITokenService tokenService)
        {
            _service = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Retrieves a new token for the specified user.
        /// </summary>
        /// <param name="apiTokenRequest">The request containing the user's information (last tokens).</param>
        /// <returns>A new set of tokens for the user.</returns>
        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh([FromBody] ApiTokenRequest apiTokenRequest)
        {
            if (apiTokenRequest is null)
                return BadRequest("Invalid client request");

            var principal = _tokenService.GetPrincipalFromExpiredToken(apiTokenRequest.AccessToken);
            var emailClaim = principal.FindFirstValue(ClaimTypes.Email);
            if (emailClaim is null)
                return StatusCode(500, "An unknow error occurred while processing the request.");

            var user = await _service.GetUserByEmail(emailClaim);

            if (user is null)
                return StatusCode(500, "An unknown error occurred while processing the request.");

            if (user.RefreshToken != apiTokenRequest.RefreshToken)
                return Unauthorized("Invalid refresh token");

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
                return Unauthorized("Refresh token has expired");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;

            try
            {
                await _service.PutUser(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the generation of a new token. Please retry.");
            }

            return Ok(new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpirationTime = user.RefreshTokenExpiryTime,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// Revokes the current token for the user.
        /// </summary>
        /// <returns>A response indicating the success of the operation.</returns>
        [HttpPost]
        [Route("revoke")]
        [Authorize(Roles = "Member, Administrator")]
        public async Task<ActionResult> Revoke()
        {
            var emailClaim = User.FindFirstValue(ClaimTypes.Email);
            if (emailClaim is null)
                return StatusCode(500, "An unknow error occurred while processing the request.");
            var user = await _service.GetUserByEmail(emailClaim);
            if (user == null) return BadRequest();

            user.RefreshToken = null;
            try
            {
                await _service.PutUser(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the generation of a new token. Please retry.");
            }
            return NoContent();
        }
    }
}
