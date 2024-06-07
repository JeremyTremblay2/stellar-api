using StellarApi.Model.Geometry;

namespace StellarApi.Model.Space
{
    /// <summary>
    /// Represents a planet in the universe.
    /// </summary>
    public class Planet : CelestialObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the planet has water.
        /// </summary>
        public bool IsWater { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the planet supports life.
        /// </summary>
        public bool IsLife { get; private set; }

        /// <summary>
        /// Gets or sets the type of the planet.
        /// </summary>
        public PlanetType PlanetType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Planet"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the planet.</param>
        /// <param name="name">The name of the planet.</param>
        /// <param name="description">The description of the planet.</param>
        /// <param name="image">The image path of the planet.</param>
        /// <param name="mass">The mass of the planet.</param>
        /// <param name="temperature">The temperature of the planet.</param>
        /// <param name="radius">The radius of the planet.</param>
        /// <param name="userAuthorId">The user author identifier of the planet.</param>
        /// <param name="isWater">A value indicating whether the planet has water.</param>
        /// <param name="isLife">A value indicating whether the planet supports life.</param>
        /// <param name="planetType">The type of the planet.</param>
        /// <param name="creationDate">The creation date of the planet.</param>
        /// <param name="modificationDate">The last modification date of the planet.</param>
        /// <param name="mapId">The unique identifier of the map.</param>
        public Planet(int id, string name, string description, string image, Position? position, double mass, double temperature, 
            double radius, int userAuthorId, bool isWater, bool isLife, PlanetType planetType, DateTime? creationDate = null, DateTime? modificationDate = null, int? mapId = null)
            : base(id, name, description, image, position, mass, temperature, radius, userAuthorId, creationDate, modificationDate, mapId)
        {
            IsWater = isWater;
            IsLife = isLife;
            PlanetType = planetType;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Id} - Planet {Name}, (Description: {Description}), Mass: {Mass}, Position: {Position}, Temperature: {Temperature}, Radius: {Radius}, Image: {Image}, CreationDate: {CreationDate}, ModificationDate: {ModificationDate}, IsWater: {IsWater}, IsLife: {IsLife}, PlanetType: {PlanetType}";
        }
    }
}
