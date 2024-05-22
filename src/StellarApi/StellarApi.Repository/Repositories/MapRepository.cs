using Microsoft.EntityFrameworkCore;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.EntityToModel;

namespace StellarApi.Repository.Repositories;

/// <summary>
/// Represents a repository for managing maps in the database.
/// </summary>
public class MapRepository : IMapRepository
{
    /// <inheritdoc/>
    public async Task<bool> AddMap(Map map)
    {
        using SpaceDbContext context = new();
        if (context.Maps is null) return false;

        await context.Maps.AddAsync(map.ToEntity());
        return await context.SaveChangesAsync() == 1;
    }

    /// <inheritdoc/>
    public async Task<bool> EditMap(int id, Map map)
    {
        using SpaceDbContext context = new();
        if (context.Maps is null) return false;
        var existingMap = await context.Maps.FindAsync(id);
        if (existingMap == null)
            return false;
        var entity = map.ToEntity();
        entity.Id = id;
        context.Maps.Update(entity);
        return await context.SaveChangesAsync() == 1;
    }

    /// <inheritdoc/>
    public async Task<Map?> GetMap(int id)
    {
        using SpaceDbContext context = new();
        if (context.Maps is null) return null;
        return (await context.Maps.FindAsync(id)).ToModel();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Map>> GetMaps(int page, int pageSize)
    {
        using SpaceDbContext context = new();
        if (context.Maps is null) return new List<Map>();
        return await context.Maps
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(map => map.ToModel())
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveMap(int id)
    {
        using SpaceDbContext context = new();
        if (context.Maps is null) return false;
        var map = await context.Maps.FindAsync(id);
        if (map is null) return false;
        context.Maps.Remove(map);
        return await context.SaveChangesAsync() == 1;
    }
}