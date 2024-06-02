using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        /// Logger used to log information in the controller.
        /// </summary>
        private readonly ILogger<TokenController> _logger;

        /// <summary>
        /// Creates a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        /// <param name="logger">The logger used to log information in the controller.</param>
        /// <param name="userService">The user service for the application.</param>
        /// <param name="tokenService">The token service for the application.</param>
        public TokenController(ILogger<TokenController> logger, IUserService userService, ITokenService tokenService)
        {
            _logger = logger;
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
            _logger.LogInformation("Refreshing the user's token.");
            if (apiTokenRequest is null)
            {
                _logger.LogInformation("Invalid client request.");
                return BadRequest("Invalid client request.");
            }

            User? user;
            string? emailClaim = string.Empty;

            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(apiTokenRequest.AccessToken);
                emailClaim = principal.FindFirstValue(ClaimTypes.Email);
                if (emailClaim is null)
                {
                    _logger.LogWarning("An unknow error occurred while getting the claims of the connecgted user.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurred while processing the request. Check that you are still connected to the application and retry.");
                }

                user = await _service.GetUserByEmail(emailClaim);
                if (user is null)
                {
                    _logger.LogWarning($"An unknown error occurred while processing the request and getting the user's information for {emailClaim}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while processing the request and getting your information.");
                }
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogInformation($"The token provided is invalid for user {emailClaim}. More details: {ex.Message}.");
                return Unauthorized("The token provided is invalid.");
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user {emailClaim} could not be found in the database due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unknown error occurred while getting the user's information for {emailClaim}. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while getting the user's information.");
            }

            if (user.RefreshToken != apiTokenRequest.RefreshToken)
            {
                _logger.LogInformation($"The refresh token provided is invalid for user {emailClaim}.");
                return Unauthorized("The refresh token provided is invalid.");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                _logger.LogInformation($"The refresh token provided has expired for user {emailClaim}.");
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
                    _logger.LogError($"An unknown error occurred while renewing the user's privileges for {emailClaim}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while renewing your user privileges.");
                }
                _logger.LogInformation($"The user {emailClaim} has successfully refreshed their token.");
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
                _logger.LogError($"The user {emailClaim} could not be found in the database due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while renewing the user's privileges for {emailClaim}. More details: {ex.Message}.");
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
                _logger.LogWarning("An unknow error occurred while getting the claims of the connected user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurred while processing the request. Check that you are still connected to the application and retry.");
            }
            _logger.LogInformation($"Revoking the user's token for {emailClaim}.");

            User? user;

            try
            {
                user = await _service.GetUserByEmail(emailClaim);
                if (user is null)
                {
                    _logger.LogError($"An unknown error occurred while processing the request and getting the user's information for {emailClaim}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while processing the request and getting your information.");
                }
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user {emailClaim} could not be found in the database due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception)
            {
                _logger.LogError($"An unknown error occurred while getting the user's information for {emailClaim}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while getting the user's information.");
            }

            try
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                var wasEdited = await _service.PutUser(user, false);
                if (!wasEdited)
                {
                    _logger.LogError($"An unknown error occurred while revoking the user's privileges for {emailClaim}.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred while revoking your user privileges.");
                }
                _logger.LogInformation($"The user {emailClaim} has successfully revoked their token.");
                return NoContent();
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user {emailClaim} could not be found in the database due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while revoking the user's privileges for {emailClaim}. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while revoking your user privileges.", Details = ex.Message });
            }
        }
    }
}
