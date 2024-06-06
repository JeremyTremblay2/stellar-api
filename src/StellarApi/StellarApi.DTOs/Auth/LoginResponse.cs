using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents the response object for a login operation.
    /// </summary>
    [SwaggerSchema("The response of the server after the user was succesfully login, containing access tokens for the user.", ReadOnly = true)]
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        [SwaggerSchema(Description = "The access token of the user to allow him to connect himself to the application.", Nullable = false)]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [SwaggerSchema(Description = "The refresh token of the user to allow him to get a new sets of tokens if its access token expires.", Nullable = false)]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the refresh token.
        /// </summary>
        [SwaggerSchema(Description = "The date when the refresh token of the user will expire.", Nullable = false)]
        public DateTime? RefreshTokenExpirationTime { get; set; }

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
    }
}
