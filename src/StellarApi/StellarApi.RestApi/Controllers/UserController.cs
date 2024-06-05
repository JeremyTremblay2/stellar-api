using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.Infrastructure.Business;
using StellarApi.DTOtoModel;
using DTOtoModel;
using StellarApi.Model.Users;
using StellarApi.RestApi.Auth;
using Microsoft.AspNetCore.Authorization;
using StellarApi.DTOs.Users;
using StellarApi.Business.Exceptions;
using StellarApi.Repository.Exceptions;

namespace StellarApi.RestApi.Controllers
{
    /// <summary>
    /// API controller for managing users.
    /// </summary>
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/users/")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The service for managing user objects.
        /// </summary>
        private readonly IUserService _service;

        /// <summary>
        /// The service for managing the tokens.
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Logger used to log information in the controller.
        /// </summary>
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Represents the number of days before a refresh token expires.
        /// </summary>
        private const int daysToExpire = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">The logger used to log information in the controller.</param>
        /// <param name="userService">The service for managing user objects.</param>
        /// <param name="tokenService">The service for managing the tokens.</param>
        public UserController(ILogger<UserController> logger, IUserService userService, ITokenService tokenService)
        {
            _logger = logger;
            _service = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The request containing the user's information.</param>
        /// <returns>The created user object wrapped in an <see cref="ActionResult"/>.</returns>
        [MapToApiVersion(1)]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> PostUser([FromBody] RegistrationRequest request)
        {
            _logger.LogInformation($"New registration request for a user: {request.Email} named {request.Username}.");
            try
            {
                User? userObject = new User(request.Email, request.Username, request.Password);
                var wasAdded = await _service.PostUser(userObject);
                if (wasAdded)
                {
                    _logger.LogInformation($"User {request.Email} added successfully.");
                    return Ok("User added successfully.");
                }
                else
                {
                    _logger.LogError($"The user {request.Email} could not be added due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user could not be added due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidEmailFormatException ||
                                   ex is InvalidFieldLengthException)
            { 
                _logger.LogInformation($"The user {request.Email} could not be added due to an invalid field: {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (DuplicateUserException ex)
            {
                _logger.LogInformation($"The user {request.Email} could not be added because a user already exists with this email. More details: {ex.Message}.");
                return Conflict(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user {request.Email} could not be added due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while adding a new user. More details:; {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while adding a new user.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="request">The request containing the user's email and password.</param>
        /// <returns>A response containing the user's information and a token.</returns>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponse>> Authenticate([FromBody] LoginRequest request)
        {
            _logger.LogInformation($"New login request for a user: {request.Email}.");
            User? user;
            try
            {
                user = await _service.GetUserByEmail(request.Email);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user {request.Email} could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while fetching user data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching user data.", Details = ex.Message });
            }

            if (user is null)
            {
                _logger.LogInformation($"The user {request.Email} could not be found, the credentials are invalid.");
                return BadRequest("Bad credentials");
            }

            // Add BCrypt password hashing here to compare the hashed password with the one in the database.
            var isPasswordValid = request.Password != null && request.Email != null
                && request.Password.Equals(user.Password) && request.Email.Equals(user.Email);
            //var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                _logger.LogInformation($"The user {request.Email} could not be authenticated, the credentials are invalid.");
                return BadRequest("Bad credentials");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(daysToExpire);

            try
            {
                var wasEdited = await _service.PutUser(user.Id, user, false);
                if (wasEdited)
                {
                    _logger.LogInformation($"The user {request.Email} was successfully authenticated and his token updated.");
                    return Ok(new LoginResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        RefreshTokenExpirationTime = user.RefreshTokenExpiryTime,
                        Email = user.Email,
                        Username = user.Username,
                        Role = user.Role.ToString()
                    });
                }
                else
                {
                    _logger.LogError($"The user {request.Email} could not be authenticated due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user could not be edited to add the refresh token due to an unknown error.");
                }
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user {request.Email} could not be authenticated due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while updating the refresh token. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while updating the refresh token.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a user object by its ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user object wrapped in an <see cref="ActionResult"/>.</returns>
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        [Authorize(Roles = "Member, Administrator")]
        public async Task<ActionResult<UserOutput?>> GetUserById(int id)
        {
            _logger.LogInformation($"Fetching user data for user n°{id}.");
            try
            {
                var result = await _service.GetUserById(id);
                if (result == null)
                {
                    _logger.LogInformation($"The user n°{id} could not be found.");
                    return NotFound();
                }
                _logger.LogInformation($"The user n°{id} was found and fetched successfully.");
                return Ok(result.ToDTO());
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user n°{id} could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while fetching user data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching user data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a list of users.
        /// </summary>
        /// <param name="page">The page number of the users to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>The list of users wrapped in an <see cref="ActionResult"/>.</returns>
        [MapToApiVersion(1)]
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<UserOutput>>> GetUsers(int page, int pageSize)
        {
            _logger.LogInformation($"Fetching user data for page {page} with {pageSize} users per page.");
            try
            {
                var users = (await _service.GetUsers(page, pageSize)).ToDTO();
                _logger.LogInformation($"User data fetched successfully.");
                return Ok(users);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user data could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while fetching user data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching user data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="user">The user object to update.</param>
        /// <returns>Returns <see cref="OkResult"/> if the user is successfully updated. Returns <see cref="BadRequestResult"/> if the user object is invalid.</returns>
        [MapToApiVersion(1)]
        [HttpPut]
        [Authorize(Roles = "Member, Administrator")]
        [Route("edit/{id}")]
        public async Task<ActionResult> PutUser(int id, [FromBody] UserInput user)
        {
            _logger.LogInformation($"Editing user data for user n°{id}.");
            User userObject = user.ToModel();
            try
            {
                var wasEdited = await _service.PutUser(id, userObject, true);
                if (wasEdited)
                {
                    _logger.LogInformation($"User n°{id} edited successfully.");
                    return Ok("User edited successfully.");
                }
                else
                {
                    _logger.LogError($"The user n°{id} could not be edited due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user could not be edited due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidEmailFormatException ||
                                   ex is InvalidFieldLengthException)
            {
                _logger.LogInformation($"The user n°{id} could not be edited due to an invalid field: {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (DuplicateUserException ex)
            {
                _logger.LogInformation($"The user n°{id} could not be edited because a user already exists with this email. More details: {ex.Message}.");
                return Conflict(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation($"The user n°{id} could not be edited because it could not be found.");
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user n°{id} could not be edited due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while editing the user's information. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while editing the user's information.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user by its ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>Returns <see cref="OkResult"/> if the user is successfully deleted. Returns <see cref="NotFoundResult"/> if the user is not found.</returns>
        [MapToApiVersion(1)]
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        [Route("remove")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            _logger.LogInformation($"Deleting user data for user n°{id}.");
            try
            {
                if (await _service.DeleteUser(id))
                {
                    _logger.LogInformation($"User n°{id} deleted successfully.");
                    return Ok($"The User n°{id} was successfully deleted.");
                }
                else
                {
                    _logger.LogError($"The user n°{id} could not be deleted due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"The user n°{id} could not be deleted due to an unknown error.");
                }
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation($"The user n°{id} could not be deleted because it could not be found.");
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The user n°{id} could not be deleted due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while deleting user data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while deleting user data.", Details = ex.Message });
            }
        }
    }
}
