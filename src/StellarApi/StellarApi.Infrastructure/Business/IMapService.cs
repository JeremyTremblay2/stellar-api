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
    /// <returns>A task that represents the asynchronous operation and returns the retrieved map, or null if not found.</returns>
    Task<Map?> GetMap(int id);

    /// <summary>
    /// Retrieves a collection of maps with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task that represents the asynchronous operation and returns the collection of maps.</returns>
    Task<IEnumerable<Map>> GetMaps(int page, int pageSize);

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
    /// <param name="id">The unique identifier of the map to delete.</param>
    /// <returns>A task that represents the asynchronous operation and returns true if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteMap(int id);
}