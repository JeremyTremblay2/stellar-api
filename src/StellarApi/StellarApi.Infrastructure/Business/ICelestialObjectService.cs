using StellarApi.Model.Space;

namespace StellarApi.Infrastructure.Business
{
    /// <summary>
    /// Represents a service for managing celestial objects.
    /// </summary>
    public interface ICelestialObjectService
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
        /// Adds a new celestial object.
        /// </summary>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
        Task<bool> PostCelestialObject(CelestialObject celestialObject);

        /// <summary>
        /// Updates an existing celestial object.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
        Task<bool> PutCelestialObject(int id, CelestialObject celestialObject);

        /// <summary>
        /// Deletes a celestial object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to delete.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteCelestialObject(int id);
    }
}
