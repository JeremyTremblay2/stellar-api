namespace StellarApi.DTOs;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a map, including its properties.
/// </summary>
public class MapDTO
{
    /// <summary>
    /// Gets or sets the unique identifier of the map.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the map.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the celestial objects in the map.
    /// </summary>
    public List<CelestialObjectDTO>? CelestialObjects { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the map.
    /// </summary>
    public DateTime? CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the last modification date of the map.
    /// </summary>
    public DateTime? ModificationDate { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapDTO"/> class with specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    /// <param name="name">The name of the map.</param>
    /// <param name="celestialObjects">The celestial objects in the map.</param>
    /// <param name="creationDate">The creation date of the map.</param>
    /// <param name="modificationDate">The last modification date of the map.</param>
    public MapDTO(int id, string name, List<CelestialObjectDTO>? celestialObjects, DateTime? creationDate, DateTime? modificationDate)
    {
        Id = id;
        Name = name;
        CelestialObjects = celestialObjects;
        CreationDate = creationDate;
        ModificationDate = modificationDate;
    }
}