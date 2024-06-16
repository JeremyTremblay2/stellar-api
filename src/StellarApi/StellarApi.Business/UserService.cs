using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Users;
using StellarApi.Helpers;
using StellarApi.Business.Exceptions;
using Microsoft.Extensions.Logging;

namespace StellarApi.Business
{
    /// <summary>
    /// Represents a service responsible for managing users.
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Constant that represents the maximum length of a username.
        /// </summary>
        private const int MaxLengthUsername = 30;

        /// <summary>
        /// Constant that represents the maximum length of an email.
        /// </summary>
        private const int MaxLengthEmail = 100;

        /// <summary>
        /// The repository used by this service.
        /// </summary>
        private readonly IUserRepository _repository;

        /// <summary>
        /// Logger used by this service.
        /// </summary>
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="logger">The logger used by this service.</param>
        /// <param name="userRepository">The repository used by this service.</param>
        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _repository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _repository.GetUserByEmail(email);
        }        

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize)
        {
            return await _repository.GetUsers(page, pageSize);
        }

        /// <inheritdoc/>
        public async Task<int> GetUsersCount()
        {
            return await _repository.GetUsersCount();
        }

        /// <inheritdoc/>
        public async Task<bool> PostUser(User user)
        {
            user.CreationDate = DateTime.Now;
            user.ModificationDate = DateTime.Now;
            await CheckUserData(0, user, 0, false);
            return await _repository.AddUser(user);
        }

        /// <inheritdoc/>
        public async Task<bool> PutUser(int id, User user, int userAuthorId, bool shouldUpdateModificationDate)
        {
            if (shouldUpdateModificationDate)
            {
                user.ModificationDate = DateTime.Now;
                await CheckUserData(id, user, userAuthorId, true);
            }
            return await _repository.EditUser(id, user);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteUser(int id, int userAuthorId)
        {
            var user = await _repository.GetUserById(userAuthorId);
            if (user != null && user.Id != id && user.Role != Role.Administrator)
            {
                throw new UnauthorizedAccessException("The user is not an administrator and cannot delete another user.");
            }
            return await _repository.RemoveUser(id);
        }

        /// <summary>
        /// Checks if the user data is valid.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="user">The user to check.</param>
        /// <param name="userAuthorId">The unique identifier of the user who is editing.</param>
        /// <param name="isEditing">A boolean indicating whether the user is being edited.</param>
        /// <exception cref="ArgumentNullException">If the user is null.</exception>
        /// <exception cref="ArgumentException">If the email or username or password is null or empty.</exception>
        /// <exception cref="InvalidEmailFormatException">If the email format is invalid.</exception>
        /// <exception cref="InvalidFieldLengthException">If the email or username is greater than the maximum length.</exception>
        /// <exception cref="DuplicateUserException">If the email is already used.</exception>
        /// <exception cref="UnauthorizedAccessException">If the user is not an administrator and tries to edit another user.</exception>"
        private async Task CheckUserData(int id, User user, int userAuthorId, bool isEditing)
        {
            if (user == null)
            {
                _logger.LogWarning("The user was null while checking its values.");
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(user.Email));
            }
            if (!user.Email.IsValidEmail())
            {
                throw new InvalidEmailFormatException("The email address has not a valid format.", nameof(user.Email));
            }
            if (user.Email.Length > MaxLengthEmail)
            {
                throw new InvalidFieldLengthException($"The email address cannot be greater than {MaxLengthEmail} characters.", nameof(user.Username));
            }
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException("The username cannot be null or empty.", nameof(user.Username));
            }
            if (user.Username.Length > MaxLengthUsername)
            {
                throw new InvalidFieldLengthException($"The username cannot be greater than {MaxLengthUsername} characters.", nameof(user.Username));
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("The password cannot be null or empty.", nameof(user.Password));
            }
            var existingUser = await _repository.GetUserByEmail(user.Email);
            if (isEditing && existingUser != null && user.Id != userAuthorId && user.Role != Role.Administrator)
            {
                throw new UnauthorizedAccessException("The user is not an administrator and cannot edit another user.");
            }
            if (existingUser != null && existingUser.Id != id)
            {
                _logger.LogTrace($"The email address {user.Email} is already used by user n°{existingUser.Id} and cannot be edited for user n°{user.Id}.");
                throw new DuplicateUserException("The email address is already used.");
            }
        }
    }
}
