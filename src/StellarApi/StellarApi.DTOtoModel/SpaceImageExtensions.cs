using StellarApi.DTOs.Space;
using StellarApi.Model.Space;

namespace StellarApi.DTOtoModel;

/// <summary>
/// Extension methods for mapping between <see cref="SpaceImage"/> and <see cref="SpaceImageDTO"/>.
/// </summary>
public static class SpaceImageExtensions
{
    /// <summary>
    /// Converts a <see cref="SpaceImageDTO"/> to a <see cref="SpaceImage"/>.
    /// </summary>
    /// <param name="dto">The SpaceImageDTO to convert.</param>
    /// <returns>The converted SpaceImage.</returns>
    /// <exception cref="ArgumentException">Thrown when the title, image or shooting date is null or empty.</exception>
    public static SpaceImage ToModel(this SpaceImageDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("The title cannot be null or empty.", nameof(dto.Title));
        }

        if (string.IsNullOrWhiteSpace(dto.Image))
        {
            throw new ArgumentException("The image url cannot be null or empty.", nameof(dto.Image));
        }

        if (dto.ShootingDate == DateTime.MinValue)
        {
            throw new ArgumentException("The shooting date cannot be empty.", nameof(dto.ShootingDate));
        }

        return new SpaceImage(0, dto.Title, dto.Description, dto.Image, dto.ShootingDate);
    }

    /// <summary>
    /// Converts a collection of <see cref="SpaceImageDTO"/> to <see cref="SpaceImage"/>.
    /// </summary>
    /// <param name="dtos">The collection of SpaceImageDTO to convert.</param>
    /// <returns>The converted collection of SpaceImages.</returns>
    public static IEnumerable<SpaceImage> ToModel(this IEnumerable<SpaceImageDTO> dtos)
    {
        return dtos.Select(dto => dto.ToModel()).Where(obj => obj != null).Select(obj => obj!);
    }

    /// <summary>
    /// Converts a <see cref="SpaceImage"/> to a <see cref="SpaceImageDTO"/>.
    /// </summary>
    /// <param name="model">The SpaceImage to convert.</param>
    /// <returns>The converted SpaceImageDTO.</returns>
    public static SpaceImageDTO ToDTO(this SpaceImage model)
    {
        return new SpaceImageDTO(model.Id, model.Title, model.Description, model.Image, model.ShootingDate);
    }

    /// <summary>
    /// Converts a collection of <see cref="SpaceImage"/> to <see cref="SpaceImageDTO"/>.
    /// </summary>
    /// <param name="models">The collection of SpaceImage to convert.</param>
    /// <returns>The converted collection of SpaceImageDTO.</returns>
    public static IEnumerable<SpaceImageDTO> ToDTO(this IEnumerable<SpaceImage> models)
    {
        return models.Select(model => model.ToDTO()).Where(obj => obj != null).Select(obj => obj!);
    }
}