using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Repository;

/// <summary>
/// Represents a repository for managing space images in a database.
/// </summary>
public interface ISpaceImageRepository
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
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>A task that represents the asynchronous operation and returns the collection of space images.</returns>
    Task<IEnumerable<SpaceImage>> GetSpaceImages(int page, int pageSize);

    /// <summary>
    /// Adds a new space image to the database.
    /// </summary>
    /// <param name="spaceImage"></param>
    /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
    Task<bool> AddSpaceImage(SpaceImage spaceImage);

    /// <summary>
    /// Retrieves the space image of the day.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the space image of the day.</returns>
    Task<SpaceImage> GetSpaceImageOfTheDay();
}