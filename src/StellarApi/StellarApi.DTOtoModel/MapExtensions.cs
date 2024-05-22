using StellarApi.DTOs;
using StellarApi.Model.Space;

namespace StellarApi.DTOtoModel;

/// <summary>
/// Extension methods for converting between <see cref="Map"/> and <see cref="MapDTO"/>.
/// </summary>
public static class MapExtensions
{
    /// <summary>
    /// Converts a <see cref="MapDTO"/> to a <see cref="Map"/>.
    /// </summary>
    /// <param name="dto">The DTO to transform.</param>
    /// <returns>The new Map object.</returns>
    public static Map ToModel(this MapDTO? dto)
        => dto is null ? null : new Map(dto.Id, dto.Name, dto.CreationDate, dto.ModificationDate);

    /// <summary>
    /// Converts a <see cref="Map"/> to a <see cref="MapDTO"/>.
    /// </summary>
    /// <param name="model">The Map to transform.</param>
    /// <returns>The new DTO Map object.</returns>
    public static MapDTO ToDTO(this Map? model)
        => model is null
            ? null
            : new MapDTO(model.Id, model.Name, new List<CelestialObjectDTO>(), model.CreationDate,
                model.ModificationDate);

    public static IEnumerable<Map> ToModel(this IEnumerable<MapDTO> dtos)
        => dtos.Select(dto => dto.ToModel());

    public static IEnumerable<MapDTO> ToDTO(this IEnumerable<Map> models)
        => models.Select(model => model.ToDTO());
}