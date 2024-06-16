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
    /// Retrieves the total number of space images in the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the total number of space images.</returns>
    /// <exception cref="UnavailableDatabaseException">If the database is not available.</exception>
    Task<int> GetSpaceImagesCount();

    /// <summary>
    /// Retrieves a space image by its shooting date.
    /// </summary>
    /// <param name="date">The shooting date of the space image.</param>
    /// <returns>A task that represents the asynchronous operation and returns the retrieved space image, or null if not found.</returns>
    Task<SpaceImage?> GetSpaceImageByDate(DateTime date);

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
    Task<SpaceImage?> GetSpaceImageOfTheDay();
}