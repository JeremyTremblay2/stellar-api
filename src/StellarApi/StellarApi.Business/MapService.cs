using Microsoft.Extensions.Logging;
using StellarApi.Business.Exceptions;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Exceptions;

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
    /// The repository used for managing celestial objects.
    /// </summary>
    private readonly ICelestialObjectRepository _celestialObjectRepository;

    /// <summary>
    /// Logger used by this service.
    /// </summary>
    private readonly ILogger<IMapService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapService"/> class.
    /// </summary>
    /// <param name="repository">The repository used for accessing maps.</param>
    public MapService(ILogger<IMapService> logger, IMapRepository repository, ICelestialObjectRepository celestialObjectRepository)
    {
        _logger = logger;
        _repository = repository;
        _celestialObjectRepository = celestialObjectRepository;
    }

    /// <inheritdoc/>
    public async Task<Map?> GetMap(int id, int? userRequestId)
    {
        var map = await _repository.GetMap(id);
        if (map != null && !map.IsPublic && (userRequestId == null || map.UserAuthorId != userRequestId))
        {
            throw new UnauthorizedAccessException($"You are not allowed to access the map n�{id} because this is not yours.");
        }
        if (map != null && map.UserAuthorId != userRequestId)
        {
            var toRemove = map.CelestialObjects.Where(c => !c.IsPublic && c.UserAuthorId != userRequestId).ToList();
            map.RemoveCelestialObjects(toRemove);
        }
        return map;
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Map>> GetMaps(int userId, int page, int pageSize)
    {
        return _repository.GetMaps(userId, page, pageSize);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Map>> GetMaps(int page, int pageSize)
    {
        return _repository.GetPublicMaps(page, pageSize);
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
    public async Task<bool> PutMap(int id, Map map)
    {
        var existingMap = await _repository.GetMap(id);
        if (existingMap != null && existingMap.UserAuthorId != map.UserAuthorId)
        {
            throw new UnauthorizedAccessException($"You are not allowed to modify the map n�{id} because this is not yours.");
        }
        CheckMapData(map);
        map.ModificationDate = DateTime.Now;
        return await _repository.EditMap(id, map);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteMap(int mapId, int userAuthorId)
    {
        var existingMap = await _repository.GetMap(mapId);
        if (existingMap != null && existingMap.UserAuthorId != userAuthorId)
        {
            throw new UnauthorizedAccessException($"You are not allowed to delete the map n�{mapId} because this is not yours.");
        }
        return await _repository.RemoveMap(mapId);
    }

    /// <inheritdoc/>
    public async Task<bool> AddCelestialObject(int mapId, int celestialObjectId, int userAuthorId)
    {
        await CheckObjectOwners(mapId, celestialObjectId, userAuthorId, true);
        return await _repository.AddCelestialObject(mapId, celestialObjectId);
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveCelestialObject(int mapId, int celestialObjectId, int userAuthorId)
    {
        await CheckObjectOwners(mapId, celestialObjectId, userAuthorId, false);
        return await _repository.RemoveCelestialObject(mapId, celestialObjectId);
    }

    /// <summary>
    /// Checks the data of the map.
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <exception cref="ArgumentNullException">If the map is null.</exception>
    /// <exception cref="ArgumentException">If the name is null or empty.</exception>
    /// <exception cref="InvalidFieldLengthException">If the name is too long.</exception>
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

    /// <summary>
    /// Checks if the user is the owner of the map and the celestial object.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object.</param>
    /// <param name="userAuthorId">The unique identifier of the user who created the map.</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException">If the user is not the owner of the map or the celestial object.</exception>
    private async Task CheckObjectOwners(int mapId, int celestialObjectId, int userAuthorId, bool toBeAdded)
    {
        var existingMap = await _repository.GetMap(mapId);
        var existingCelestialObject = await _celestialObjectRepository.GetCelestialObject(celestialObjectId);
        if (existingCelestialObject == null)
        {
            throw new EntityNotFoundException(celestialObjectId.ToString(), "The celestial object was not found.");
        }
        if (existingMap == null)
        {
            throw new EntityNotFoundException(mapId.ToString(), "The map was not found.");
        }
        if (existingCelestialObject.UserAuthorId != userAuthorId)
        {
            throw new UnauthorizedAccessException($"You are not allowed to add the celestial object n�{celestialObjectId} to the map n�{mapId} because this celestial object is not yours.");
        }
        if (existingMap.UserAuthorId != userAuthorId)
        {
            throw new UnauthorizedAccessException($"You are not allowed to add a celestial object to the map n�{mapId} because this map is not yours.");
        }
        if (toBeAdded && existingMap.CelestialObjects.Contains(existingCelestialObject))
        {
            throw new CelestialObjectAlreadyInMapException($"The celestial object n�{celestialObjectId} is already in the map n�{mapId} and cannot be linked to it.");
        }
        if (!toBeAdded && !existingMap.CelestialObjects.Contains(existingCelestialObject))
        {
            throw new CelestialObjectNotInMapException($"The celestial object n�{celestialObjectId} is not in the map n�{mapId} and cannot be removed from it.");
        }
    }
}