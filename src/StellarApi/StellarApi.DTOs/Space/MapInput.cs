using Swashbuckle.AspNetCore.Annotations;

namespace StellarApi.DTOs.Space;

/// <summary>
/// Represents a Data Transfer Object (DTO) for a map in input (less data than the complete object), including its properties.
/// </summary>
[SwaggerSchema("The request object to create a map.", ReadOnly = true)]
public class MapInput
{
    /// <summary>
    /// Gets or sets the name of the map.
    /// </summary>
    [SwaggerSchema("The name of the map.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapOutput"/> class with specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    public MapInput(string name)
    {
        Name = name;
    }
}