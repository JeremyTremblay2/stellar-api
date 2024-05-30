namespace StellarApi.Model.Users
{
    /// <summary>
    /// Represents a user with an email, a name, a password and date information about the account.
    /// </summary>
    public class User : IEquatable<User>, IComparable<User>, IComparable
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the password of the user.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the user's role.
        /// </summary>
        public Role Role { get; private set; }

        /// <summary>
        /// Gets the creation date of the user.
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Gets the modification date of the user.
        /// </summary>
        public DateTime ModificationDate { get; private set; }

        /// <summary>
        /// The refresh token of the user.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// The expiry time of the refresh token.
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="role">The role of the user.</param>
        public User(string email, string username, string password, Role role)
        {
            CheckUserData(email, username, password);
            Role = role;
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <param name="creationDate">The creation date of the user.</param>
        /// <param name="modificationDate">The modification date of the user.</param>
        public User(int id, string email, string username, string password, Role role, string? refreshToken = null, 
            DateTime? refreshTokenExpiryTime = null, DateTime? creationDate = null, DateTime? modificationDate = null)
        {
            Id = id;
            Role = role;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
            CheckUserData(email, username, password);
            CheckDates(creationDate, modificationDate);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(User)) return false;
            return Equals((User)obj);
        }

        /// <inheritdoc/>
        public bool Equals(User? other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }

        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not User)
                throw new ArgumentException("Object is not a User", nameof(obj));
            return CompareTo((User)obj);
        }

        /// <inheritdoc/>
        public int CompareTo(User? other)
        {
            if (other == null) return 1;
            return string.Compare(Username, other.Username, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override string? ToString()
            => $"{Id} - {Username} - {Role}, (Email: {Email}), CreationDate: {CreationDate}, ModificationDate: {ModificationDate}";

        /// <summary>
        /// Checks the user data and sets the email, username and password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <exception cref="ArgumentException">If the user email, the username or or his password is null or empty.</exception>
        private void CheckUserData(string email, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            Email = email;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Checks the dates and sets the creation and modification dates.
        /// </summary>
        /// <param name="modificationDate">The modification date to be set.</param>
        /// <param name="creationDate">The creation date to be set.</param>
        /// <exception cref="ArgumentException">If the creation date is before the modification date or if the dates are in the future.</exception>
        private void CheckDates(DateTime? creationDate, DateTime? modificationDate)
        {
            if (modificationDate.HasValue)
            {
                if (modificationDate.Value > DateTime.Now)
                    throw new ArgumentException("The modification date cannot be in the future.", nameof(modificationDate));
                else if (creationDate.HasValue && modificationDate.Value < creationDate.Value)
                    throw new ArgumentException("The modification date cannot be before the creation date.", nameof(modificationDate));
                ModificationDate = modificationDate.Value;
            }
            else
                ModificationDate = DateTime.Now;

            if (creationDate.HasValue)
            {
                if (creationDate.Value > DateTime.Now)
                    throw new ArgumentException("The creation date cannot be in the future.", nameof(creationDate));
                CreationDate = creationDate.Value;
            }
            else
            {
                CreationDate = DateTime.Now;
                ModificationDate = DateTime.Now;
            }
        }
    }
}
