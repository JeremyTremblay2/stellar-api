namespace StellarApi.DTOs
{
    /// <summary>
    /// Data transfer object for a user.
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets the password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets the role of the user.
        /// </summary>
        public string Role { get; set; } 

        /// <summary>
        /// Gets the creation date of the user.
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets the modification date of the user.
        /// </summary>
        public DateTime? ModificationDate { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="UserDTO"/> class.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <param name="creationDate">The creation date of the user.</param>
        /// <param name="modificationDate">The modificaiton date of the user.</param>
        public UserDTO(int id, string email, string username, string password, string role, DateTime? creationDate, DateTime? modificationDate)
        {
            Id = id;
            Email = email;
            Username = username;
            Password = password;
            Role = role;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
        }
    }
}
