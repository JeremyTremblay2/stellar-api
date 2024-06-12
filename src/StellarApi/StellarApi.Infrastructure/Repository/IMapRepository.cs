using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Repository;

/// <summary>
/// Represents a repository for managing maps in the database.
/// </summary>
public interface IMapRepository
{
    /// <summary>
    /// Retrieves a map by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    /// <returns>A task that represents the asynchronous operation and returns the retrieved map, or null if not found.</returns>
    Task<Map> GetMap(int id);

    /// <summary>
    /// Retrieves a collection of private maps for the specified user with pagination.
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
    Task<IEnumerable<Map>> GetPublicMaps(int page, int pageSize);

    /// <summary>
    /// Adds a new map to the data store.
    /// </summary>
    /// <param name="map">The map to add.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
    Task<bool> AddMap(Map map);

    /// <summary>
    /// Updates an existing map in the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the map to update.</param>
    /// <param name="map">The map with updated information.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
    Task<bool> EditMap(int id, Map map);

    /// <summary>
    /// Removes a map from the data store by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the map to remove.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the removal was successful; otherwise, false.</returns>
    Task<bool> RemoveMap(int id);

    /// <summary>
    /// Adds a celestial object to a map.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to add.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
    Task<bool> AddCelestialObject(int mapId, int celestialObjectId);

    /// <summary>
    /// Removes a celestial object from a map.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to remove.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the removal was successful; otherwise, false.</returns>
    Task<bool> RemoveCelestialObject(int mapId, int celestialObjectId);
}