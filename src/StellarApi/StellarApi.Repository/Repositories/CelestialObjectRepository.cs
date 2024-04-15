using Microsoft.EntityFrameworkCore;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.EntityToModel;

namespace StellarApi.Repository.Repositories
{
    /// <summary>
    /// Represents a repository for managing celestial objects in the database.
    /// </summary>
    public class CelestialObjectRepository : ICelestialObjectRepository
    {
        /// <summary>
        /// Adds a new celestial object to the database.
        /// </summary>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
        public async Task<bool> AddCelestialObject(CelestialObject celestialObject)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return false;

            await context.CelestialObjects.AddAsync(celestialObject.ToEntity());
            return await context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Updates an existing celestial object in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
        public async Task<bool> EditCelestialObject(int id, CelestialObject celestialObject)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return false;
            var existingCelestialObject = await context.CelestialObjects.FindAsync(id);
            if (existingCelestialObject == null)
                return false;
            var entity = celestialObject.ToEntity();
            entity.Id = id;
            context.CelestialObjects.Update(entity);
            return await context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Retrieves a celestial object by its unique identifier from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>A task that represents the asynchronous operation and returns the retrieved celestial object, or null if not found.</returns>
        public async Task<CelestialObject?> GetCelestialObject(int id)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return null;
            return (await context.CelestialObjects.FindAsync(id)).ToModel();
        }

        /// <summary>
        /// Retrieves a collection of celestial objects with pagination from the database.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation and returns the collection of celestial objects.</returns>
        public async Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize)
        {
            var celestialObjects = new List<CelestialObject>();
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return celestialObjects;

            return (await context.CelestialObjects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync())
                .ToModel();
        }

        /// <summary>
        /// Removes a celestial object from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to remove.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the removal was successful; otherwise, false.</returns>
        public async Task<bool> RemoveCelestialObject(int id)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return false;
            var celestialObject = await context.CelestialObjects.FindAsync(id);
            if (celestialObject == null) return false;

            context.CelestialObjects.Remove(celestialObject);
            return await context.SaveChangesAsync() == 1;
        }
    }
}
