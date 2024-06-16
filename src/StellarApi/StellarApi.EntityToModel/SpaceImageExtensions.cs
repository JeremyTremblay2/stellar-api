using StellarApi.Entities;
using StellarApi.Model.Space;

namespace StellarApi.EntityToModel;

/// <summary>
/// Provides extension methods for converting between <see cref="SpaceImageEntity"/> and <see cref="SpaceImage"/>.
/// </summary>
public static class SpaceImageExtensions
{
    /// <summary>
    /// Converts a <see cref="SpaceImageEntity"/> to a <see cref="SpaceImage"/>.
    /// </summary>
    /// <param name="entity">The entity to transform.</param>
    /// <returns>A model representing the entity object.</returns>
    public static SpaceImage ToModel(this SpaceImageEntity? entity)
    {
        if (entity is null) return null;
        return new SpaceImage(entity.Id, entity.Title, entity.Description, entity.Image, entity.ShootingDate);
    }

    /// <summary>
    /// Converts a collection of <see cref="SpaceImageEntity"/> to <see cref="SpaceImage"/>.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public static IEnumerable<SpaceImage> ToModel(this IEnumerable<SpaceImageEntity> entities)
    {
        return entities.Select(entity => entity.ToModel()).Where(obj => obj != null).Select(obj => obj!);
    }

    /// <summary>
    /// Converts a <see cref="SpaceImage"/> to a <see cref="SpaceImageEntity"/>.
    /// </summary>
    /// <param name="model">The model to transform.</param>
    /// <returns>An entity representing the model object.</returns>
    public static SpaceImageEntity ToEntity(this SpaceImage model)
    {
        return new SpaceImageEntity
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            Image = model.Image,
            ShootingDate = model.ShootingDate
        };
    }

    /// <summary>
    /// Converts a collection of <see cref="SpaceImage"/> to <see cref="SpaceImageEntity"/>.
    /// </summary>
    /// <param name="models">The collection of SpaceImage to convert.</param>
    /// <returns>The converted collection of SpaceImageEntity.</returns>
    public static IEnumerable<SpaceImageEntity> ToEntity(this IEnumerable<SpaceImage> models)
    {
        return models.Select(model => model.ToEntity());
    }
}