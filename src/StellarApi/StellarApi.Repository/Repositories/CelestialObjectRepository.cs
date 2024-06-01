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
        /// Represents the database context for managing celestial object data.
        /// </summary>
        private readonly SpaceDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for managing celestial object data.</param>
        public CelestialObjectRepository(SpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new celestial object to the database.
        /// </summary>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
        public async Task<bool> AddCelestialObject(CelestialObject celestialObject)
        {
            if (_context.CelestialObjects is null) return false;

            await _context.CelestialObjects.AddAsync(celestialObject.ToEntity());
            return await _context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Updates an existing celestial object in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
        public async Task<bool> EditCelestialObject(int id, CelestialObject celestialObject)
        {
            if (_context.CelestialObjects is null) return false;
            var existingCelestialObject = await _context.CelestialObjects.FindAsync(id);
            if (existingCelestialObject == null)
                return false;
            var entity = celestialObject.ToEntity();
            existingCelestialObject.Name = entity.Name;
            existingCelestialObject.Type = entity.Type;
            existingCelestialObject.Mass = entity.Mass;
            existingCelestialObject.Radius = entity.Radius;
            existingCelestialObject.Temperature = entity.Temperature;
            existingCelestialObject.Description = entity.Description;
            existingCelestialObject.CreationDate = entity.CreationDate;
            existingCelestialObject.ModificationDate = entity.ModificationDate;
            existingCelestialObject.Image = entity.Image;
            existingCelestialObject.Position = entity.Position;
            existingCelestialObject.StarType = entity.StarType;
            existingCelestialObject.PlanetType = entity.PlanetType;
            existingCelestialObject.IsLife = entity.IsLife;
            existingCelestialObject.IsWater = entity.IsWater;
            existingCelestialObject.Brightness = entity.Brightness;
            _context.CelestialObjects.Update(existingCelestialObject);
            return await _context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Retrieves a celestial object by its unique identifier from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>A task that represents the asynchronous operation and returns the retrieved celestial object, or null if not found.</returns>
        public async Task<CelestialObject?> GetCelestialObject(int id)
        {
            if (_context.CelestialObjects is null) return null;
            return (await _context.CelestialObjects.FindAsync(id)).ToModel();
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
            if (_context.CelestialObjects is null) return celestialObjects;

            return (await _context.CelestialObjects
                .Skip(page * pageSize)
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
            if (_context.CelestialObjects is null) return false;
            var celestialObject = await _context.CelestialObjects.FindAsync(id);
            if (celestialObject == null) return false;

            _context.CelestialObjects.Remove(celestialObject);
            return await _context.SaveChangesAsync() == 1;
        }
    }
}
