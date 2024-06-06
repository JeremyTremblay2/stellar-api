using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents a registration request.
    /// </summary>
    [SwaggerSchema("The registration request made by a user to create an account in the application.", ReadOnly = true)]
    public class RegistrationRequest
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "The username of the user.", Nullable = false)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "The encrypted password of the user.", Nullable = false)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [EmailAddress]
        [SwaggerSchema(Description = "The email adress of the user.", Nullable = false)]
        public string Email { get; set; }
    }
}
