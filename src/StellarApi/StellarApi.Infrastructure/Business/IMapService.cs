using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Business;

/// <summary>
/// Represents a service for managing maps.
/// </summary>
public interface IMapService
{
    /// <summary>
    /// Retrieves a map by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    /// <param name="userRequestId">The unique identifier of the user requesting the map.</param>
    /// <returns>A task that represents the asynchronous operation and returns the retrieved map, or null if not found.</returns>
    Task<Map?> GetMap(int id, int? userRequestId);

    /// <summary>
    /// Retrieves a collection of private maps from the specified user with pagination.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task that represents the asynchronous operation and returns the collection of maps.</returns>
    Task<IEnumerable<Map>> GetMaps(int userId, int page, int pageSize);

    /// <summary>
    /// Retrieves a collection of public maps with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task that represents the asynchronous operation and returns the collection of maps.</returns>
    Task<IEnumerable<Map>> GetMaps(int page, int pageSize);

    /// <summary>
    /// Retrieves the total number of maps for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation and returns the total number of maps.</returns>
    Task<int> GetMapsCount(int userId);

    /// <summary>
    /// Retrieves the total number of maps.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the total number of maps.</returns>
    Task<int> GetPublicMapsCount();

    /// <summary>
    /// Adds a new map.
    /// </summary>
    /// <param name="map">The map to add.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
    Task<bool> PostMap(Map map);

    /// <summary>
    /// Updates an existing map.
    /// </summary>
    /// <param name="id">The unique identifier of the map to update.</param>
    /// <param name="map">The map with updated information.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
    Task<bool> PutMap(int id, Map map);

    /// <summary>
    /// Deletes a map by its unique identifier.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map to delete.</param>
    /// <param name="userAuthorId">The unique identifier of the user who created the map.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteMap(int mapId, int userAuthorId);

    /// <summary>
    /// Adds a celestial object to a map.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to add.</param>
    /// <param name="userAuthorId">The unique identifier of the user who created the map.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
    Task<bool> AddCelestialObject(int mapId, int celestialObjectId, int userAuthorId);

    /// <summary>
    /// Removes a celestial object from a map.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to remove.</param>
    /// <param name="userAuthorId">The unique identifier of the user who created the map.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the removal was successful; otherwise, false.</returns>
    Task<bool> RemoveCelestialObject(int mapId, int celestialObjectId, int userAuthorId);
}