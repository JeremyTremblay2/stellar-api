using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.Infrastructure.Business;
using StellarApi.DTOtoModel;
using DTOtoModel;
using StellarApi.Model.Users;
using StellarApi.RestApi.Auth;
using Microsoft.AspNetCore.Authorization;
using StellarApi.DTOs.Users;

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
            User? userObject = null;
            try
            {
                userObject = new User(request.Email, request.Username, request.Password, Role.Member);
                _ = userObject ?? throw new Exception("The object was not created.");
            }
            catch (ArgumentException err)
            {
                return BadRequest(err.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the conversion of the object. Please retry.");
            }

            if (await _service.PostUser(userObject))
            {
                return CreatedAtAction(nameof(PostUser), userObject);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the addition of the object. Please retry.");
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
            var user = await _service.GetUserByEmail(request.Email);
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
                await _service.PutUser(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the generation of a new token. Please retry.");
            }

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
            var result = await _service.GetUserById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result.ToDTO());
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
            var users = (await _service.GetUsers(page, pageSize)).ToDTO();
            return Ok(users);
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
            User? userObject = null;
            try
            {
                userObject = user.ToModel();
                _ = userObject ?? throw new Exception("The object was not created.");
            }
            catch (ArgumentException err)
            {
                return BadRequest(err.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the conversion of the object. Please retry.");
            }

            if (await _service.PutUser(userObject))
            {
                return Ok($"The User {user.Username} was successfully edited.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the edition of the object. Please retry.");
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
            if (await _service.DeleteUser(id))
            {
                return Ok($"The User n°{id} was successfully deleted.");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
