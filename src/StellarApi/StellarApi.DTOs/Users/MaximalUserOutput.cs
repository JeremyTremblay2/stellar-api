using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.DTOs.Users
{
    /// <summary>
    /// Ouput data transfer object for a user containing more private information.
    /// </summary>
    [SwaggerSchema("The maximal output data transfer object for a user, used when a user is returned by the server, containing more private information about him.", ReadOnly = true)]
    public class MaximalUserOutput
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [SwaggerSchema(Description = "The unique identifier of the user.", Nullable = false)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [EmailAddress]
        [SwaggerSchema(Description = "The email adress of the user.", Nullable = false)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [SwaggerSchema(Description = "The username of the user.", Nullable = false)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        [SwaggerSchema(Description = "The role of the user.", Nullable = false)]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the user.
        /// </summary>
        [SwaggerSchema(Description = "The date when the user was created.", Nullable = false)]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the modification date of the user.
        /// </summary>
        [SwaggerSchema(Description = "The date when the user was last modified.", Nullable = false)]
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="MaximalUserOutput"/> class.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <param name="creationDate">The creation date of the user.</param>
        /// <param name="modificationDate">The modificaiton date of the user.</param>
        public MaximalUserOutput(int id, string username, string email, string role, DateTime creationDate, DateTime modificationDate)
        {
            Id = id;
            Username = username;
            Email = email;
            Role = role;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
        }
    }
}
