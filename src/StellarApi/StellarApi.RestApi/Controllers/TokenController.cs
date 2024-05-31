using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Users;
using StellarApi.Repository.Exceptions;
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
            {
                return BadRequest("Invalid client request.");
            }

            User? user;

            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(apiTokenRequest.AccessToken);
                var emailClaim = principal.FindFirstValue(ClaimTypes.Email);
                if (emailClaim is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurred while processing the request. Check that you are still connected to the application and retry.");
                }

                user = await _service.GetUserByEmail(emailClaim);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while processing the request and getting your information.");
                }
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while getting the user's information.");
            }

            if (user.RefreshToken != apiTokenRequest.RefreshToken)
            {
                return Unauthorized("The refresh token provided is invalid.");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized("The refresh token provided has expired.");
            }

            try
            {
                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                var wasEdited = await _service.PutUser(user, false);
                if (!wasEdited)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while revoking your user privileges.");
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
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while revoking your user privileges.", Details = ex.Message });
            }
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
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurred while processing the request. Check that you are still connected to the application and retry.");
            }

            User? user;

            try
            {
                user = await _service.GetUserByEmail(emailClaim);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while processing the request and getting your information.");
                }
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while getting the user's information.");
            }

            try
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                var wasEdited = await _service.PutUser(user, false);
                if (!wasEdited)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while revoking your user privileges.");
                }
                return NoContent();
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while revoking your user privileges.", Details = ex.Message });
            }
        }
    }
}
