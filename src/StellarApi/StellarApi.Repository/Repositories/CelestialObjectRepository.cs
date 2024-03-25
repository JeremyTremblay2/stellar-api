using Microsoft.EntityFrameworkCore;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;

namespace StellarApi.Repository.Repositories
{
    public class CelestialObjectRepository : ICelestialObjectRepository
    {
        public async Task<bool> AddCelestialObject(CelestialObject celestialObject)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return false;

            await context.CelestialObjects.AddAsync(celestialObject);
            return await context.SaveChangesAsync() == 1;
        }

        public async Task<bool> EditCelestialObject(int id, CelestialObject celestialObject)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return false;
            var existingCelestialObject = await context.CelestialObjects.FindAsync(id);
            if (existingCelestialObject == null)
                return false;

            existingCelestialObject.Name = celestialObject.Name;
            existingCelestialObject.Description = celestialObject.Description;
            existingCelestialObject.Mass = celestialObject.Mass;
            existingCelestialObject.Temperature = celestialObject.Temperature;
            existingCelestialObject.Radius = celestialObject.Radius;
            existingCelestialObject.Image = celestialObject.Image;
            existingCelestialObject.CreationDate = celestialObject.CreationDate;
            existingCelestialObject.ModificationDate = celestialObject.ModificationDate;

            return await context.SaveChangesAsync() == 1;
        }

        public async Task<CelestialObject?> GetCelestialObject(int id)
        {
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return null;
            return await context.CelestialObjects.FindAsync(id);
        }

        public async Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize)
        {
            var celestialObjects = new List<CelestialObject>();
            using SpaceDbContext context = new();
            if (context.CelestialObjects is null) return celestialObjects;

            return await context.CelestialObjects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

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
