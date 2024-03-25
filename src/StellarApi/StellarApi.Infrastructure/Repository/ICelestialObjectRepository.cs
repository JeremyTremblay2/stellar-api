using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository for managing celestial objects in a data store.
    /// </summary>
    public interface ICelestialObjectRepository
    {
        /// <summary>
        /// Retrieves a celestial object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>A task that represents the asynchronous operation and returns the retrieved celestial object, or null if not found.</returns>
        Task<CelestialObject?> GetCelestialObject(int id);

        /// <summary>
        /// Retrieves a collection of celestial objects with pagination.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation and returns the collection of celestial objects.</returns>
        Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize);

        /// <summary>
        /// Adds a new celestial object to the data store.
        /// </summary>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
        Task<bool> AddCelestialObject(CelestialObject celestialObject);

        /// <summary>
        /// Updates an existing celestial object in the data store.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
        Task<bool> EditCelestialObject(int id, CelestialObject celestialObject);

        /// <summary>
        /// Removes a celestial object from the data store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to remove.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the removal was successful; otherwise, false.</returns>
        Task<bool> RemoveCelestialObject(int id);
    }
}
