using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StellarApi.EntityToModel;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.Repository.Exceptions;

namespace StellarApi.Repository.Repositories;

public class SpaceImageRepository : ISpaceImageRepository
{
    /// <summary>
    /// Represents the database context for managing space image data.
    /// </summary>
    private readonly SpaceDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for managing space image data.</param>
    public SpaceImageRepository(SpaceDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<SpaceImage?> GetSpaceImage(int id)
    {
        if (_context.SpaceImages is null) throw new UnavailableDatabaseException();
        return (await _context.SpaceImages.FindAsync(id)).ToModel();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SpaceImage>> GetSpaceImages(int page, int pageSize)
    {
        if (_context.SpaceImages is null) throw new UnavailableDatabaseException();
        return (await _context.SpaceImages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync())
            .ToModel();
    }

    /// <inheritdoc/>
    public async Task<bool> AddSpaceImage(SpaceImage spaceImage)
    {
        if (_context.SpaceImages is null) throw new UnavailableDatabaseException();
        _context.SpaceImages.Add(spaceImage.ToEntity());
        return await _context.SaveChangesAsync() == 1;
    }

    /// <inheritdoc/>
    public async Task<SpaceImage> GetSpaceImageOfTheDay()
    {
        if (_context.SpaceImages is null) throw new UnavailableDatabaseException();
        return (await _context.SpaceImages
                .Where(s => s.ShootingDate == DateTime.Today)
                .FirstOrDefaultAsync())
            .ToModel();
    }
}