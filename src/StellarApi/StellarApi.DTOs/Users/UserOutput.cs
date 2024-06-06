using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.DTOs.Users
{
    /// <summary>
    /// Ouput data transfer object for a user.
    /// </summary>
    [SwaggerSchema("The output data transfer object for a user, used when a user is returned by the server, containing information about him.", ReadOnly = true)]
    public class UserOutput
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        [SwaggerSchema(Description = "The unique identifier of the user.", Nullable = false)]
        public int Id { get; set; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        [EmailAddress]
        [SwaggerSchema(Description = "The email adress of the user.", Nullable = false)]
        public string Email { get; set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        [SwaggerSchema(Description = "The username of the user.", Nullable = false)]
        public string Username { get; set; }

        /// <summary>
        /// Gets the role of the user.
        /// </summary>
        [SwaggerSchema(Description = "The role of the user.", Nullable = false)]
        public string Role { get; set; }

        /// <summary>
        /// Gets the creation date of the user.
        /// </summary>
        [SwaggerSchema(Description = "The date when the user was created.", Nullable = false)]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets the modification date of the user.
        /// </summary>
        [SwaggerSchema(Description = "The date when the user was last modified.", Nullable = false)]
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="UserOutput"/> class.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <param name="creationDate">The creation date of the user.</param>
        /// <param name="modificationDate">The modificaiton date of the user.</param>
        public UserOutput(int id, string email, string username, string role, DateTime creationDate, DateTime modificationDate)
        {
            Id = id;
            Email = email;
            Username = username;
            Role = role;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
        }
    }
}
