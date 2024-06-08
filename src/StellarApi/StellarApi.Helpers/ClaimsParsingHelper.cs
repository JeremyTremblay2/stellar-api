namespace StellarApi.Helpers;

/// <summary>
/// Represents a class that can be helpful during the process of claims parsing.
/// </summary>
public static class ClaimsParsingHelper
{
    /// <summary>
    /// Parses the user ID from the claims.
    /// </summary>
    /// <param name="userId">The user ID to parse.</param>
    /// <returns>A nullable integer representing the user ID.</returns>
    public static int? ParseUserId(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        if (int.TryParse(userId, out var parsedUserId))
        {
            return parsedUserId;
        }

        return null;
    }
}
