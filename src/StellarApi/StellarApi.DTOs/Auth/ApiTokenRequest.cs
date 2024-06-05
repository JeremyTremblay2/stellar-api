using Swashbuckle.AspNetCore.Annotations;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents the request object for an API token.
    /// </summary>
    [SwaggerSchema("The request object to get a new set of API tokens.", ReadOnly = true)]
    public class ApiTokenRequest
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        [SwaggerSchema(Description = "The access token of the user (expire or not).", Nullable = false)]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [SwaggerSchema(Description = "The refresh token of the user (should not be expired).", Nullable = false)]
        public required string RefreshToken { get; set; }
    }
}
