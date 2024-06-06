using StellarApi.DTOs;
using StellarApi.DTOs.Space;
using StellarApi.Model.Space;

namespace StellarApi.DTOtoModel;

/// <summary>
/// Extension methods for converting between <see cref="Map"/> and <see cref="MapOutput"/>.
/// </summary>
public static class MapExtensions
{
    /// <summary>
    /// Converts a <see cref="MapInput"/> to a <see cref="Map"/>.
    /// </summary>
    /// <param name="dto">The DTO to transform.</param>
    /// <returns>The new Map object.</returns>
    public static Map ToModel(this MapInput? dto)
        => dto is null ? null : new Map(0, dto.Name);

    /// <summary>
    /// Converts a <see cref="Map"/> to a <see cref="MapOutput"/>.
    /// </summary>
    /// <param name="model">The Map to transform.</param>
    /// <returns>The new DTO Map object.</returns>
    public static MapOutput ToDTO(this Map? model)
        => model is null
            ? null
            : new MapOutput(model.Id, model.Name, model.CelestialObjects.ToDTO(), model.CreationDate,
                model.ModificationDate);

    /// <summary>
    /// Converts a collection of <see cref="MapInput"/> to a collection of <see cref="Map"/>.
    /// </summary>
    /// <param name="dtos">The list of DTOs to transform.</param>
    /// <returns>A new list of Map.</returns>
    public static IEnumerable<Map> ToModel(this IEnumerable<MapInput> dtos)
        => dtos.Select(dto => dto.ToModel());

    /// <summary>
    /// Converts a collection of <see cref="Map"/> to a collection of <see cref="MapOutput"/>.
    /// </summary>
    /// <param name="models">The list of Map to transform.</param>
    /// <returns>The new list of Map DTO.</returns>
    public static IEnumerable<MapOutput> ToDTO(this IEnumerable<Map> models)
        => models.Select(model => model.ToDTO());
}