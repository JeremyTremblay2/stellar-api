using System.ComponentModel.DataAnnotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents a registration request.
    /// </summary>
    public class RegistrationRequest
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
