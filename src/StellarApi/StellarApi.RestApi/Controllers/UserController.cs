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
using System.Security.Claims;
using StellarApi.Helpers;
using System.Net.Http.Headers;

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
        /// (Needs Auth) Retrieves complete data of the connected user.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to retrieve a the complete data of the connected user (more data than the minimal user output).
        /// 
        /// In the response, the user's password is not included for security reasons.
        /// 
        /// Sample request:
        /// 
        ///     GET /api/v1/users/me
        ///     
        /// </remarks>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user object data.</returns>
        [MapToApiVersion(1)]
        [HttpGet("me")]
        [Authorize(Roles = "Member, Administrator")]
        [ProducesResponseType<MaximalUserOutput>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<MaximalUserOutput?>> GetCurrentUser()
        {
            _logger.LogInformation($"Fetching complete user data for connected user.");
            try
            {
                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }
                var result = await _service.GetUserById(userId.Value);
                if (result == null)
                {
                    _logger.LogInformation($"The connected user n°{userId} could not be found.");
                    return NotFound();
                }
                _logger.LogInformation($"The connected user n°{userId} was found and fetched successfully.");
                return Ok(result.ToMaximalDTO());
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The connected user could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while fetching the connected user data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching data of the connected user.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a user object by its unique identifier.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to retrieve a user object with its data by its unique identifier.
        /// 
        /// In the response, the user's password is not included for security reasons.
        /// 
        /// A 404 error will be returned if the user is not found.
        /// 
        /// Sample request:
        /// 
        ///     GET /api/v1/users/1
        ///     
        /// </remarks>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user object data.</returns>
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType<MinimalUserOutput>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<MinimalUserOutput?>> GetUserById(int id)
        {
            _logger.LogInformation($"Fetching user data for user n°{id}.");
            try
            {
                var result = await _service.GetUserById(id);
                if (result == null)
                {
                    _logger.LogInformation($"The user n°{id} could not be found.");
                    return NotFound($"The user n°{id} could not be found.");
                }
                _logger.LogInformation($"The user n°{id} was found and fetched successfully.");
                return Ok(result.ToMinimalDTO());
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
        /// <remarks>
        /// 
        /// This route is used to retrieve a list of users with their data.
        /// 
        /// The page and page size parameters are mandatory and must be greater than 0.
        /// 
        /// In the response list, the user's password and user passwords are not included for security reasons.
        /// 
        /// A 400 error will be returned if the page or page size is invalid.
        /// 
        /// Sample request:
        /// 
        ///     GET /api/v1/users?page=1&amp;pageSize=10
        /// 
        /// </remarks>
        /// <param name="page">The page number of the users to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>The list of users and the associated data.</returns>
        [MapToApiVersion(1)]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType<List<MinimalUserOutput>>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<MinimalUserOutput>>> GetUsers(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Fetching user data for page {page} with {pageSize} users per page.");

            if (page <= 0)
            {
                _logger.LogInformation($"User data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
                return BadRequest("The page number must be greater than 0.");
            }
            if (pageSize <= 0)
            {
                _logger.LogInformation($"User data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
                return BadRequest("The page size must be greater than 0.");
            }

            try
            {
                var users = (await _service.GetUsers(page, pageSize)).ToMinimalDTO();
                var totalUsers = await _service.GetUsersCount();
                var firstItemIndex = (page - 1) * pageSize;
                var lastItemIndex = firstItemIndex + users.Count() - 1;
                Response.Headers["Content-Range"] = $"users {firstItemIndex}-{lastItemIndex}/{totalUsers}";
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
        /// Creates a new user.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to create a new user with the given information.
        /// 
        /// All the parameters in the request body are mandatory.
        /// 
        /// The username must have a maximum length of 30 characters and the email a maximum length of 100 characters.
        /// 
        /// A 400 error will be returned if the user information contains invalid data.
        /// 
        /// A 409 error will be returned if a user is already registered with the same email address.
        /// 
        /// **DO NOT** include the user's password in plain text in the request body. Instead, make sure to hash the password before calling this method.
        ///  
        /// Sample request:
        ///
        ///     POST /api/v1/users/register
        ///     {
        ///         "username": "JohnDoe",
        ///         "password": "f4k3p4ssw0rd",
        ///         "email": "johndoe@example.com"
        ///     }
        ///     
        /// </remarks>
        /// <param name="request">The request containing the user's information.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        [MapToApiVersion(1)]
        [HttpPost]
        [Route("register")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
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
        /// Authenticates a user and give him the credentials to connects him to the API.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to authenticate a user and generate an access token and a refresh token for the user to use when accessing the API.
        /// 
        /// Once connected, the user can use the access token to access the API and the refresh token to generate a new access token when the current one expires.
        /// 
        /// A 400 error will be returned if the user credentials are invalid.
        /// 
        /// **DO NOT** include the user's password in plain text in the request body. Instead, make sure to hash the password before calling this route.
        /// 
        /// **DO NOT** expose the refresh token to the client-side. The refresh token should be securely stored on the server-side and only used to generate new access tokens when needed.
        /// 
        /// Sample request:
        ///
        ///     POST /api/v1/users/login
        ///     {
        ///         "email": "johndoe@example.com",
        ///         "password": "f4k3p4ssw0rd"
        ///     }
        ///     
        /// </remarks>
        /// <param name="request">The request containing the user's email and password.</param>
        /// <returns>A response containing the user's information its connexion a token.</returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
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
                var wasEdited = await _service.PutUser(user.Id, user, user.Id, false);
                if (wasEdited)
                {
                    _logger.LogInformation($"The user {request.Email} was successfully authenticated and his token updated.");
                    return Ok(new LoginResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        RefreshTokenExpirationTime = user.RefreshTokenExpiryTime,
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
        /// (Needs Auth) Updates an existing user.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to update an existing user with new data. This can be used to update the user's email, username, or password.
        /// 
        /// The username must have a maximum length of 30 characters and the email a maximum length of 100 characters. The id is mandatory.
        /// 
        /// Only a user can update his own account, an administrator can update any account.
        /// 
        /// A 400 error will be returned if the user information contains invalid data.
        /// 
        /// A 403 error will be returned if the user is not allowed to update the user.
        /// 
        /// A 404 will be returned if the user to update is not found.
        /// 
        /// A 409 will be returned if a user already exists with the same email, it means that two users cannot exist with the same email.
        /// 
        /// **DO NOT** include the user's password in plain text in the request body. Instead, make sure to hash the password before calling this route.
        /// 
        /// Sample request:
        /// 
        ///     PUT /api/v1/users/edit
        ///     {
        ///         "id": 1,
        ///         "username": "JohnDoe",
        ///         "password": "f4k3p4ssw0rd",
        ///         "email": "johndoe@example.com"
        ///     }
        /// 
        /// </remarks>
        /// <param name="user">The user object to update.</param>
        /// <returns>Returns a response indicating if the user was updated or not.</returns>
        [MapToApiVersion(1)]
        [HttpPut]
        [Authorize(Roles = "Member, Administrator")]
        [Route("edit/{id}")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> PutUser(int id, [FromBody] UserInput user)
        {
            _logger.LogInformation($"Editing user data for user n°{id}.");
            User userObject = user.ToModel();
            try
            {
                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }
                var wasEdited = await _service.PutUser(id, userObject, userId.Value, true);
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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"The user n°{id} could not be edited because the user is not allowed to delete it. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
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
        /// (Needs Auth) Deletes a user by its unique identifier.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to deleter a user object by its unique identifier. The data cannot be recovered once deleted.
        /// 
        /// Only the user can delete his own account, an administrator can delete any account.
        /// 
        /// A 403 error will be returned if the user is not allowed to delete the account.
        /// 
        /// A 404 error will be returned if the user is not found.
        /// 
        /// Sample request:
        /// 
        ///     DELETE /api/v1/users/remove/1
        /// 
        /// </remarks>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>Return a message indicating if the user was deleted or not.</returns>
        [MapToApiVersion(1)]
        [HttpDelete]
        [Authorize(Roles = "Member, Administrator")]
        [Route("remove/{id}")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            _logger.LogInformation($"Deleting user data for user n°{id}.");
            try
            {
                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }
                if (await _service.DeleteUser(id, userId.Value))
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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation($"The user n°{id} could not be deleted because the user is not allowed to delete it. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
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
