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
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets the modification date of the user.
        /// </summary>
        public DateTime ModificationDate { get; set; }

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
        public User(string email, string username, string password, Role role = Role.Member)
        {
            Email = email;
            Username = username;
            Password = password;
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
        public User(int id, string email, string username, string password, Role role = Role.Member, string? refreshToken = null, 
            DateTime? refreshTokenExpiryTime = null, DateTime? creationDate = null, DateTime? modificationDate = null)
        {
            Id = id;
            Email = email;
            Username = username;
            Password = password;
            Role = role;
            CreationDate = creationDate ?? DateTime.Now;
            ModificationDate = modificationDate ?? DateTime.Now;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
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
    }
}
