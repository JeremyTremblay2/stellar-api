namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents the request object for an API token.
    /// </summary>
    public class ApiTokenRequest
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public required string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public required string RefreshToken { get; set; }
    }
}
