using Swashbuckle.AspNetCore.Annotations;

namespace StellarApi.DTOs.Space;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a map in output (more data than the input object), including its properties.
/// </summary>
[SwaggerSchema("The request object to get a map.", ReadOnly = true)]
public class MapOutput
{
    /// <summary>
    /// Gets or sets the unique identifier of the map.
    /// </summary>
    [SwaggerSchema("The unique identifier of the map.")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the map.
    /// </summary>
    [SwaggerSchema("The name of the map.")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the author of the map.
    /// </summary>
    [SwaggerSchema(Description = "The unique identifier of the author of the map.", Nullable = true)]
    public int? UserAuthorId { get; set; }

    /// <summary>
    /// Gets or sets the celestial objects in the map.
    /// </summary>
    [SwaggerSchema("The celestial objects presents in the map.")]
    public List<CelestialObjectOutput>? CelestialObjects { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the map.
    /// </summary>
    [SwaggerSchema("The creation date of the map.")]
    public DateTime? CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the last modification date of the map.
    /// </summary>
    [SwaggerSchema("The last modification date of the map.")]
    public DateTime? ModificationDate { get; set; }

    /// <summary>
    /// Gets or sets the public status of the map.
    /// </summary>
    [SwaggerSchema(Description = "A value indicating if the map is visible by everyone or not.", Nullable = true)]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapOutput"/> class with specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    /// <param name="name">The name of the map.</param>
    /// <param name="isPublic">The public status of the map.</param>
    /// <param name="userAuthorId">The unique identifier of the author of the map.</param>
    /// <param name="celestialObjects">The celestial objects in the map.</param>
    /// <param name="creationDate">The creation date of the map.</param>
    /// <param name="modificationDate">The last modification date of the map.</param>
    public MapOutput(int id, string name, bool isPublic, int userAuthorId, IEnumerable<CelestialObjectOutput>? celestialObjects, DateTime? creationDate, DateTime? modificationDate)
    {
        Id = id;
        Name = name;
        IsPublic = isPublic;
        UserAuthorId = userAuthorId;
        CelestialObjects = celestialObjects is null ? new List<CelestialObjectOutput>() : celestialObjects.ToList();
        CreationDate = creationDate;
        ModificationDate = modificationDate;
    }
}