using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents a login request.
    /// </summary>
    [SwaggerSchema("The login request made by a user to connect him to the application.", ReadOnly = true)]
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [SwaggerSchema(Description = "The email adress of the user.", Nullable = false)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "The encrypted password of the user.", Nullable = false)]
        public string Password { get; set; }
    }
}
