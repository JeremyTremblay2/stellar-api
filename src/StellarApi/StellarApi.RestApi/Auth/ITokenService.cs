using StellarApi.Model.Users;

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
        string CreateToken(User user);

        /// <summary>
        /// Gets the role of the specified user.
        /// </summary>
        /// <param name="user">The user whose role is retrieved.</param>
        /// <returns>The role of the user.</returns>
        Role GetUserRole(User user);
    }
}
