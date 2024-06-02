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
        public async Task<bool> PostUser(User user)
        {
            user.CreationDate = DateTime.Now;
            user.ModificationDate = DateTime.Now;
            await CheckUserData(user);
            return await _repository.AddUser(user);
        }

        /// <inheritdoc/>
        public async Task<bool> PutUser(User user, bool shouldUpdateModificationDate)
        {
            if (shouldUpdateModificationDate)
            {
                user.ModificationDate = DateTime.Now;
            }
            await CheckUserData(user);
            return await _repository.EditUser(user);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteUser(int id)
        {
            return await _repository.RemoveUser(id);
        }

        /// <summary>
        /// Checks if the user data is valid.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="ArgumentNullException">If the user is null.</exception>
        /// <exception cref="ArgumentException">If the email or username or password is null or empty.</exception>
        /// <exception cref="InvalidEmailFormatException">If the email format is invalid.</exception>
        /// <exception cref="InvalidFieldLengthException">If the email or username is greater than the maximum length.</exception>
        /// <exception cref="DuplicateUserException">If the email is already used.</exception>
        private async Task CheckUserData(User user)
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
            if (existingUser != null && existingUser.Id != user.Id)
            {
                _logger.LogTrace($"The email address {user.Email} is already used bu user n°{existingUser.Id} and cannot be edited for user n°{user.Id}.");
                throw new DuplicateUserException("The email address is already used.");
            }
        }
    }
}
