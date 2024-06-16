using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Repository;

/// <summary>
/// Represents a repository for fetching space images from an API.
/// </summary>
public interface IHttpSpaceImageRepository
{
    /// <summary>
    /// Fetches the space image of the day from the API.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the space image of the day.</returns>
    Task<SpaceImage> FetchSpaceImageOfTheDay();
}