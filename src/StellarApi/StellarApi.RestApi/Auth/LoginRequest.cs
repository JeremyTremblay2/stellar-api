using System.ComponentModel.DataAnnotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents a login request.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
