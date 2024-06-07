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
    /// Gets or sets the public status of the map.
    /// </summary>
    [SwaggerSchema(Description = "A value indicating if the map is visible by everyone or not.", Nullable = true)]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapOutput"/> class with specified properties.
    /// </summary>
    /// <param name="name">The name of the map.</param>
    /// <param name="isPublic">The public status of the map.</param>
    public MapInput(string name, bool isPublic)
    {
        Name = name;
        IsPublic = isPublic;
    }
}