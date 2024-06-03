namespace StellarApi.DTOs.Space;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a map in input (less data than the complete object), including its properties.
/// </summary>
public class MapInput
{
    /// <summary>
    /// Gets or sets the unique identifier of the map.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the map.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the celestial objects in the map.
    /// </summary>
    public List<CelestialObjectInput>? CelestialObjects { get; set; } = new List<CelestialObjectInput>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MapOutput"/> class with specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    /// <param name="name">The name of the map.</param>
    /// <param name="celestialObjects">The celestial objects in the map.</param>
    public MapInput(int id, string name, List<CelestialObjectInput>? celestialObjects)
    {
        Id = id;
        Name = name;
        CelestialObjects = celestialObjects;
    }
}