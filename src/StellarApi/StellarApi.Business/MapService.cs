using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;

namespace StellarApi.Business;

/// <summary>
/// Service class for managing maps.
/// </summary>
public class MapService : IMapService
{
    private readonly IMapRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapService"/> class.
    /// </summary>
    /// <param name="repository">The repository used for accessing maps.</param>
    public MapService(IMapRepository repository)
    {
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
        return _repository.AddMap(map);
    }

    /// <inheritdoc/>
    public Task<bool> PutMap(int id, Map map)
    {
        return _repository.EditMap(id, map);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteMap(int id)
    {
        return _repository.RemoveMap(id);
    }
}