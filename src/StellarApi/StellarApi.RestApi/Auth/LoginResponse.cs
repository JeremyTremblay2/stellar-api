using StellarApi.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents the response object for a login operation.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the refresh token.
        /// </summary>
        public DateTime RefreshTokenExpirationTime { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public Role Role { get; set; }
    }
}
