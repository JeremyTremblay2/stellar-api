using StellarApi.Model.Geometry;
using System.Globalization;

namespace StellarApi.Model.Space
{
    /// <summary>
    /// Represents a celestial object.
    /// </summary>
    public abstract class CelestialObject : IEquatable<CelestialObject>, IComparable<CelestialObject>, IComparable
    {
        /// <summary>
        /// Gets the unique identifier of the celestial object.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The name of the celestial object.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets the name of the celestial object.
        /// </summary>
        public string Name 
        { 
            get => name; 
            set
            {
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
            }
        }

        /// <summary>
        /// Gets or sets the description of the celestial object.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets the image path of the celestial object.
        /// </summary>
        public string? Image { get; private set; }

        /// <summary>
        /// Gets or sets the position of the celestial object.
        /// </summary>
        public Position? Position { get; private set; }

        /// <summary>
        /// Gets or sets the mass of the celestial object.
        /// </summary>
        public double Mass { get; private set; }

        /// <summary>
        /// Gets or sets the temperature of the celestial object.
        /// </summary>
        public double Temperature { get; private set; }

        /// <summary>
        /// Gets or sets the radius of the celestial object.
        /// </summary>
        public double Radius { get; private set; }

        /// <summary>
        /// Gets the creation date of the celestial object.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets the last modification date of the celestial object.
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets the unique identifier of the map.
        /// </summary>
        public int? MapId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObject"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <param name="name">The name of the celestial object.</param>
        /// <param name="description">The description of the celestial object.</param>
        /// <param name="image">The image path of the celestial object.</param>
        /// <param name="position">The position of the celestial object.</param>
        /// <param name="mass">The mass of the celestial object.</param>
        /// <param name="temperature">The temperature of the celestial object.</param>
        /// <param name="radius">The radius of the celestial object.</param>
        /// <param name="creationDate">The creation date of the celestial object.</param>
        /// <param name="modificationDate">The last modification date of the celestial object.</param>
        /// <param name="mapId">The unique identifier of the map.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="radius"/> or <paramref name="mass"/> is less than or equal to 0 or when <paramref name="creationDate"/> or <paramref name="modificationDate"/> is in the future or invalid.</exception>
        public CelestialObject(int id, string name, string description, string? image, Position? position, double mass, 
            double temperature, double radius, DateTime? creationDate = null, DateTime? modificationDate = null, int? mapId = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
            Position = position;
            Mass = mass;
            Temperature = temperature;
            Radius = radius;
            CreationDate = creationDate ?? DateTime.Now;
            ModificationDate = modificationDate ?? DateTime.Now;
            MapId = mapId;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CelestialObject)) return false;
            return Equals((CelestialObject) obj);
        }

        /// <inheritdoc/>
        public bool Equals(CelestialObject? other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }

        /// <inheritdoc/>
        public int CompareTo(CelestialObject? other)
        {
            if (other == null) return 1;

            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not CelestialObject)
                throw new ArgumentException("Object is not a CelestialObject", nameof(obj));
            return CompareTo((CelestialObject)obj);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Id} - {Name}, (Description: {Description}), Position: {Position}, Mass: {Mass}, Temperature: {Temperature}, Radius: {Radius}, Image: {Image}, CreationDate: {CreationDate}, ModificationDate: {ModificationDate}";
        }
    }
}
