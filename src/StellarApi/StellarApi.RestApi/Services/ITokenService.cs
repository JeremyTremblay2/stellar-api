using StellarApi.Model.Users;
using System.Security.Claims;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Represents a token service for creating and managing tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Creates a token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is created.</param>
        /// <returns>The created token.</returns>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates a refresh token.
        /// </summary>
        /// <returns>The generated refresh token.</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Extracts the claims from the specified token.
        /// </summary>
        /// <param name="token">The token from which to extract the claims.</param>
        /// <returns>The extracted claims.</returns>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
