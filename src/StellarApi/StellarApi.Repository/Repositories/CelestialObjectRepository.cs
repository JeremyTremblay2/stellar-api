using Microsoft.EntityFrameworkCore;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.EntityToModel;
using StellarApi.Repository.Exceptions;

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
        /// Retrieves a celestial object by its unique identifier from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>A task that represents the asynchronous operation and returns the retrieved celestial object, or null if not found.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<CelestialObject?> GetCelestialObject(int id)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            return (await _context.CelestialObjects.FindAsync(id)).ToModel();
        }

        /// <summary>
        /// Retrieves a private collection of celestial objects from the specified user id with pagination from the database.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation and returns the collection of private celestial objects.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<IEnumerable<CelestialObject>> GetCelestialObjects(int userId, int page, int pageSize)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            return (await _context.CelestialObjects
                .Where(c => c.UserAuthorId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync())
                .ToModel();
        }

        /// <summary>
        /// Retrieves the total number of private celestial objects from the specified user id from the database.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation and returns the total number of private celestial objects.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<int> GetCelestialObjectsCount(int userId)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            return await _context.CelestialObjects
                .Where(c => c.UserAuthorId == userId)
                .CountAsync();
        }

        /// <summary>
        /// Retrieves a public collection of celestial objects with pagination from the database.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation and returns the collection of public celestial objects.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<IEnumerable<CelestialObject>> GetPublicCelestialObjects(int page, int pageSize)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            return (await _context.CelestialObjects
                .Where(c => c.IsPublic)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync())
                .ToModel();
        }

        /// <summary>
        /// Retrieves the total number of public celestial objects from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation and returns the total number of public celestial objects.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<int> GetPublicCelestialObjectsCount()
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            return await _context.CelestialObjects
                .Where(c => c.IsPublic)
                .CountAsync();
        }

        /// <summary>
        /// Adds a new celestial object to the database.
        /// </summary>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the addition was successful; otherwise, false.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<bool> AddCelestialObject(CelestialObject celestialObject)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            await _context.CelestialObjects.AddAsync(celestialObject.ToEntity());
            return await _context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Updates an existing celestial object in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the update was successful; otherwise, false.</returns>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when the celestial object is not found.</exception>"
        public async Task<bool> EditCelestialObject(int id, CelestialObject celestialObject)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            var existingCelestialObject = await _context.CelestialObjects.FindAsync(id);
            if (existingCelestialObject == null) throw new EntityNotFoundException(id.ToString(), "The celestial object was not found.");
            var entity = celestialObject.ToEntity();
            existingCelestialObject.Name = entity.Name;
            existingCelestialObject.Type = entity.Type;
            existingCelestialObject.Mass = entity.Mass;
            existingCelestialObject.Radius = entity.Radius;
            existingCelestialObject.Temperature = entity.Temperature;
            existingCelestialObject.Description = entity.Description;
            existingCelestialObject.ModificationDate = entity.ModificationDate;
            existingCelestialObject.Image = entity.Image;
            existingCelestialObject.Position = entity.Position;
            existingCelestialObject.StarType = entity.StarType;
            existingCelestialObject.PlanetType = entity.PlanetType;
            existingCelestialObject.IsLife = entity.IsLife;
            existingCelestialObject.IsWater = entity.IsWater;
            existingCelestialObject.Brightness = entity.Brightness;
            existingCelestialObject.MapId = entity.MapId;
            existingCelestialObject.IsPublic = entity.IsPublic;
            _context.CelestialObjects.Update(existingCelestialObject);
            return await _context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Removes a celestial object from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to remove.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the removal was successful; otherwise, false.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the celestial object is not found.</exception>
        /// <exception cref="UnavailableDatabaseException">Thrown when the database is not available.</exception>
        public async Task<bool> RemoveCelestialObject(int id)
        {
            if (_context.CelestialObjects is null) throw new UnavailableDatabaseException();
            var celestialObject = await _context.CelestialObjects.FindAsync(id);
            if (celestialObject == null) throw new EntityNotFoundException(id.ToString(), "The celestial object was not found.");
            _context.CelestialObjects.Remove(celestialObject);
            return await _context.SaveChangesAsync() == 1;
        }
    }
}
