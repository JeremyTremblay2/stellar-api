using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.DTOs.Users
{
    /// <summary>
    /// Input data transfer object for a user.
    /// </summary>
    [SwaggerSchema("The input data transfer object for a user, used when a user change its information.", ReadOnly = true)]
    public class UserInput
    {
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
        /// Gets the password of the user.
        /// </summary>
        [SwaggerSchema(Description = "The encrypted password of the user.", Nullable = false)]
        public string Password { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="UserInput"/> class.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public UserInput(string email, string username, string password)
        {
            Email = email;
            Username = username;
            Password = password;
        }
    }
}
