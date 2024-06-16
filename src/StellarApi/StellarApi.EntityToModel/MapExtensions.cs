using StellarApi.Entities;
using StellarApi.Model.Space;

namespace StellarApi.EntityToModel;

/// <summary>
/// Extension methods for converting between <see cref="Map"/> and <see cref="MapEntity"/>.
/// </summary>
public static class MapExtensions
{
    /// <summary>
    /// Converts a <see cref="MapEntity"/> to a <see cref="Map"/>.
    /// </summary>
    /// <param name="entity">The entity to transform.</param>
    /// <returns>The new Map object.</returns>
    public static Map ToModel(this MapEntity? entity)
    {
        if (entity is null) return null;
        return new Map(entity.Id, entity.Name, entity.UserAuthorId, entity.CelestialObjects.ToModel(), entity.IsPublic,
            entity.CreationDate, entity.ModificationDate);
    }

    /// <summary>
    /// Converts a <see cref="Map"/> to a <see cref="MapEntity"/>.
    /// </summary>
    /// <param name="model">The model to transform.</param>
    /// <returns>The new entity Map object.</returns>
    public static MapEntity ToEntity(this Map? model)
    {
        if (model is null) return null;
        return new MapEntity
        {
            Id = model.Id,
            Name = model.Name,
            UserAuthorId = model.UserAuthorId,
            IsPublic = model.IsPublic,
            CelestialObjects = model.CelestialObjects.ToEntity().ToList(),
            CreationDate = model.CreationDate,
            ModificationDate = model.ModificationDate
        };
    }

    /// <summary>
    /// Converts a collection of <see cref="MapEntity"/> to a collection of <see cref="Map"/>.
    /// </summary>
    /// <param name="entities">The entities to be converted.</param>
    /// <returns>A new list of Map objects.</returns>
    public static IEnumerable<Map> ToModel(this IEnumerable<MapEntity> entities)
        => entities.Select(entity => entity.ToModel());


    /// <summary>
    /// Converts a collection of <see cref="Map"/> to a collection of <see cref="MapEntity"/>.
    /// </summary>
    /// <param name="models">The Map objects to be converted.</param>
    /// <returns>A new list of MapEntity object.</returns>
    public static IEnumerable<MapEntity> ToEntity(this IEnumerable<Map> models)
        => models.Select(model => model.ToEntity());
}