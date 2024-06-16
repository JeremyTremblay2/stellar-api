using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Business;

/// <summary>
/// Represents a service for managing space images.
/// </summary>
public interface ISpaceImageService
{
    /// <summary>
    /// Retrieves a space image by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the space image.</param>
    /// <returns>A task that represents the asynchronous operation and returns the retrieved space image, or null if not found.</returns>
    Task<SpaceImage?> GetSpaceImage(int id);

    /// <summary>
    /// Retrieves a collection of space images with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task that represents the asynchronous operation and returns the collection of space images.</returns>
    Task<IEnumerable<SpaceImage>> GetSpaceImages(int page, int pageSize);

    /// <summary>
    /// Retrieves the space image of the day.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the space image of the day.</returns>
    Task<SpaceImage> GetSpaceImageOfTheDay();
}