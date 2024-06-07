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
        /// Retrieves a new set of tokens based on the user's tokens specified.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to refresh the user's token. The user must provide their current access token and refresh token to get a new set of tokens. These new tokens can then be used to access the application's resources.
        /// 
        /// The parameters are provided in the request body as a JSON object. The access token and refresh token are required to refresh the user's token.
        /// 
        /// A 400 Bad Request response is returned if the request is invalid or the tokens are not provided.
        /// 
        /// A 401 Unauthorized response is returned if the access token or refresh token is invalid.
        /// 
        /// **DO NOT** expose the refresh token to the client-side. The refresh token should be securely stored on the server-side and only used to generate new access tokens when needed.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/v1/tokens/refresh
        ///     {
        ///         "accessToken": "eyJfhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
        ///         "refreshToken": "f4bGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
        ///     }
        /// 
        /// </remarks>
        /// <param name="apiTokenRequest">The request containing the user's information (the last tokens).</param>
        /// <returns>A new set of tokens for the user.</returns>
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
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
                var wasEdited = await _service.PutUser(user.Id, user, user.Id, false);
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
        /// (Needs Auth) Revokes the current token for the connected user.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to revoke the user's token. The user must be connected to the application to revoke their token. Once called, the refresh token is removed from the user's information and the user is disconnected from the application.
        /// 
        /// This will require him to log in again to access the application's resources once its current token will be expired.
        /// 
        /// Sample request:
        ///     
        ///     POST /api/v1/tokens/revoke
        /// 
        /// </remarks>
        /// <returns>A response indicating the success of the operation.</returns>
        [HttpPost]
        [Route("revoke")]
        [Authorize(Roles = "Member, Administrator")]
        [ProducesResponseType<object>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
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
                var wasEdited = await _service.PutUser(user.Id, user, user.Id, false);
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
