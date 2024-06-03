using Microsoft.EntityFrameworkCore;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.EntityToModel;
using StellarApi.Repository.Exceptions;

namespace StellarApi.Repository.Repositories;

/// <summary>
/// Represents a repository for managing maps in the database.
/// </summary>
public class MapRepository : IMapRepository
{
    /// <summary>
    /// Represents the database _context for managing map data.
    /// </summary>
    private readonly SpaceDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for managing map data.</param>
    public MapRepository(SpaceDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<bool> AddMap(Map map)
    {
        if (_context.Maps is null) throw new UnavailableDatabaseException();

        await _context.Maps.AddAsync(map.ToEntity());
        return await _context.SaveChangesAsync() == 1;
    }

    /// <inheritdoc/>
    public async Task<bool> EditMap(int id, Map map)
    {
        if (_context.Maps is null) throw new UnavailableDatabaseException();
        var existingMap = await _context.Maps.FindAsync(id);
        if (existingMap == null) throw new EntityNotFoundException(id.ToString(), "The map was not found.");
        var entity = map.ToEntity();
        existingMap.Name = entity.Name;
        existingMap.CelestialObjects = entity.CelestialObjects;
        _context.Maps.Update(existingMap);
        return await _context.SaveChangesAsync() == 1;
    }

    /// <inheritdoc/>
    public async Task<Map?> GetMap(int id)
    {
        if (_context.Maps is null) return null;
        return (await _context.Maps.FindAsync(id)).ToModel();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Map>> GetMaps(int page, int pageSize)
    {
        if (_context.Maps is null) throw new UnavailableDatabaseException();
        return (await _context.Maps
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync())
            .ToModel();
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveMap(int id)
    {
        if (_context.Maps is null) throw new UnavailableDatabaseException();
        var map = await _context.Maps.FindAsync(id);
        if (map is null) return false;
        _context.Maps.Remove(map);
        return await _context.SaveChangesAsync() == 1;
    }
}