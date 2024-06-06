using Microsoft.Extensions.Logging;
using StellarApi.Business.Exceptions;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;

namespace StellarApi.Business;

/// <summary>
/// Service class for managing maps.
/// </summary>
public class MapService : IMapService
{
    /// <summary>
    /// Constant that represents the maximum length of a name.
    /// </summary>
    private const int MaxLengthName = 100;

    /// <summary>
    /// The repository used by this service.
    /// </summary>
    private readonly IMapRepository _repository;

    /// <summary>
    /// Logger used by this service.
    /// </summary>
    private readonly ILogger<IMapService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapService"/> class.
    /// </summary>
    /// <param name="repository">The repository used for accessing maps.</param>
    public MapService(ILogger<IMapService> logger, IMapRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    /// <inheritdoc/>
    public Task<Map?> GetMap(int id)
    {
        return _repository.GetMap(id);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Map>> GetMaps(int page, int pageSize)
    {
        return _repository.GetMaps(page, pageSize);
    }

    /// <inheritdoc/>
    public Task<bool> PostMap(Map map)
    {
        CheckMapData(map);
        map.CreationDate = DateTime.Now;
        map.ModificationDate = DateTime.Now;
        return _repository.AddMap(map);
    }

    /// <inheritdoc/>
    public Task<bool> PutMap(int id, Map map)
    {
        CheckMapData(map);
        map.ModificationDate = DateTime.Now;
        return _repository.EditMap(id, map);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteMap(int id)
    {
        return _repository.RemoveMap(id);
    }

    /// <inheritdoc/>
    public Task<bool> AddCelestialObject(int mapId, int celestialObjectId)
    {
        return _repository.AddCelestialObject(mapId, celestialObjectId);
    }

    /// <inheritdoc/>
    public Task<bool> RemoveCelestialObject(int mapId, int celestialObjectId)
    {
        return _repository.RemoveCelestialObject(mapId, celestialObjectId);
    }

    private void CheckMapData(Map map)
    {
        if (map == null)
        {
            _logger.LogWarning("The map was null while checking its data.");
            throw new ArgumentNullException(nameof(map));
        }

        if (string.IsNullOrWhiteSpace(map.Name))
        {
            throw new ArgumentException("The map name cannot be null or empty.");
        }

        if (map.Name.Length > MaxLengthName)
        {
            throw new InvalidFieldLengthException($"The map name cannot be longer than {MaxLengthName} characters.");
        }
    }
}