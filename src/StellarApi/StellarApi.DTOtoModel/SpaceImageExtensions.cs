using StellarApi.DTOs.Space;
using StellarApi.Model.Space;

namespace StellarApi.DTOtoModel;

/// <summary>
/// Extension methods for mapping between <see cref="SpaceImage"/> and <see cref="SpaceImageOutput"/>.
/// </summary>
public static class SpaceImageExtensions
{
    /// <summary>
    /// Converts a <see cref="SpaceImageOutput"/> to a <see cref="SpaceImage"/>.
    /// </summary>
    /// <param name="dto">The SpaceImageOutput to convert.</param>
    /// <returns>The converted SpaceImage.</returns>
    /// <exception cref="ArgumentException">Thrown when the title, image or shooting date is null or empty.</exception>
    public static SpaceImage ToModel(this SpaceImageOutput dto)
    {
        return new SpaceImage(0, dto.Title, dto.Description, dto.Image, dto.ShootingDate);
    }

    /// <summary>
    /// Converts a collection of <see cref="SpaceImageOutput"/> to <see cref="SpaceImage"/>.
    /// </summary>
    /// <param name="dtos">The collection of SpaceImageOutput to convert.</param>
    /// <returns>The converted collection of SpaceImages.</returns>
    public static IEnumerable<SpaceImage> ToModel(this IEnumerable<SpaceImageOutput> dtos)
    {
        return dtos.Select(dto => dto.ToModel()).Where(obj => obj != null).Select(obj => obj!);
    }

    /// <summary>
    /// Converts a <see cref="SpaceImage"/> to a <see cref="SpaceImageOutput"/>.
    /// </summary>
    /// <param name="model">The SpaceImage to convert.</param>
    /// <returns>The converted SpaceImageOutput.</returns>
    public static SpaceImageOutput ToDTO(this SpaceImage model)
    {
        return new SpaceImageOutput(model.Id, model.Title, model.Description, model.Image, model.ShootingDate);
    }

    /// <summary>
    /// Converts a collection of <see cref="SpaceImage"/> to <see cref="SpaceImageOutput"/>.
    /// </summary>
    /// <param name="models">The collection of SpaceImage to convert.</param>
    /// <returns>The converted collection of SpaceImageOutput.</returns>
    public static IEnumerable<SpaceImageOutput> ToDTO(this IEnumerable<SpaceImage> models)
    {
        return models.Select(model => model.ToDTO()).Where(obj => obj != null).Select(obj => obj!);
    }
}