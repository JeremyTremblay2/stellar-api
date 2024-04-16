using StellarApi.Model.Geometry;

namespace StellarApi.Model.Space;

/// <summary>
/// Represents a star in the universe.
/// </summary>
public class Star : CelestialObject
{
    /// <summary>
    /// Gets or sets the brightness of the star.
    /// </summary>
    public int Brightness { get; private set; }

    /// <summary>
    /// Gets or sets the type of the star.
    /// </summary>
    public StarType StarType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class with specified properties.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="image"></param>
    /// <param name="mass"></param>
    /// <param name="temperature"></param>
    /// <param name="radius"></param>
    /// <param name="brightness"></param>
    /// <param name="starType"></param>
    public Star(int id, string name, string description, string image, Position? position, double mass, double temperature, double radius, DateTime? creationDate,
        DateTime? modificationDate, int brightness, StarType starType) : base(id, name, description, image, position, mass, temperature, radius, creationDate, modificationDate)
    {
        Brightness = brightness;
        StarType = starType;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Id} - Star {Name}, (Description: {Description}), Mass: {Mass}, Temperature: {Temperature}, Radius: {Radius}, Image: {Image}, CreationDate: {CreationDate}, ModificationDate: {ModificationDate}, Brightness: {Brightness}, StarType: {StarType}";
}