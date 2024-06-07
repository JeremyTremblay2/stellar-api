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
    public double Brightness { get; private set; }

    /// <summary>
    /// Gets or sets the type of the star.
    /// </summary>
    public StarType StarType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Star"/> class with specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the star.</param>
    /// <param name="name">The name of the star.</param>
    /// <param name="description">The description of the star.</param>
    /// <param name="image">The image of the star.</param>
    /// <param name="position">The position of the star.</param>
    /// <param name="mass">The mass of the star.</param>
    /// <param name="temperature">The temperature of the star.</param>
    /// <param name="radius">The radius of the star.</param>
    /// <param name="userAuthorId">The user author identifier of the star.</param>
    /// <param name="brightness">The brightness of the star.</param>
    /// <param name="starType">The type of the star.</param>
    /// <param name="creationDate">The creation date of the star.</param>
    /// <param name="modificationDate">The last modification date of the star.</param>
    /// <param name="mapId">The unique identifier of the map.</param>
    public Star(int id, string name, string description, string image, Position? position, double mass,
        double temperature, double radius, int userAuthorId, double brightness, StarType starType, bool isPublic = false, DateTime? creationDate = null,
        DateTime? modificationDate = null, int? mapId = null) : base(id, name, description, image, position, mass, temperature, radius, userAuthorId, isPublic,
        creationDate, modificationDate, mapId)
    {
        Brightness = brightness;
        StarType = starType;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Id} - Star {Name}, (Description: {Description}), Mass: {Mass}, Position: {Position}, Temperature: {Temperature}, Radius: {Radius}, Image: {Image}, CreationDate: {CreationDate}, ModificationDate: {ModificationDate}, Brightness: {Brightness}, StarType: {StarType} - {(IsPublic ? "Public" : "Private")}";
}