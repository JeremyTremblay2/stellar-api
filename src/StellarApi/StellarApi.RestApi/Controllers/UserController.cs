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
        /// Represents the number of days before a refresh token expires.
        /// </summary>
        private const int daysToExpire = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The service for managing user objects.</param>
        /// <param name="tokenService">The service for managing the tokens.</param>
        public UserController(IUserService userService, ITokenService tokenService)
        {
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
            User? userObject = new User(request.Email, request.Username, request.Password);
            try
            {
                var wasAdded = await _service.PostUser(userObject);
                if (wasAdded)
                {
                    return Ok("User added successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user could not be added due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidEmailFormatException ||
                                   ex is InvalidFieldLengthException)
            { 
                return BadRequest(ex.Message);
            }
            catch (DuplicateUserException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
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
            User? user;
            try
            {
                user = await _service.GetUserByEmail(request.Email);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching user data.", Details = ex.Message });
            }

            if (user is null)
            {
                return BadRequest("Bad credentials");
            }

            // Add BCrypt password hashing here to compare the hashed password with the one in the database.
            var isPasswordValid = request.Password != null && request.Email != null
                && request.Password.Equals(user.Password) && request.Email.Equals(user.Email);
            //var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(daysToExpire);

            try
            {
                var wasEdited = await _service.PutUser(user, false);
                if (wasEdited)
                {
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
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user could not be edited to add the refresh token due to an unknown error.");
                }
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
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
            try
            {
                var result = await _service.GetUserById(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result.ToDTO());
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
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
            try
            {
                var users = (await _service.GetUsers(page, pageSize)).ToDTO();
                return Ok(users);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching user data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user object to update.</param>
        /// <returns>Returns <see cref="OkResult"/> if the user is successfully updated. Returns <see cref="BadRequestResult"/> if the user object is invalid.</returns>
        [MapToApiVersion(1)]
        [HttpPut]
        [Authorize(Roles = "Member, Administrator")]
        [Route("edit")]
        public async Task<ActionResult> PutUser([FromBody] UserInput user)
        {
            User userObject = user.ToModel();
            try
            {
                var wasEdited = await _service.PutUser(userObject, true);
                if (wasEdited)
                {
                    return Ok("User edited successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "The user could not be edited due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidEmailFormatException ||
                                   ex is InvalidFieldLengthException)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicateUserException ex)
            {
                return Conflict(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
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
            try
            {
                if (await _service.DeleteUser(id))
                {
                    return Ok($"The User n°{id} was successfully deleted.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"The user n°{id} could not be deleted due to an unknown error.");
                }
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while deleting user data.", Details = ex.Message });
            }
        }
    }
}
