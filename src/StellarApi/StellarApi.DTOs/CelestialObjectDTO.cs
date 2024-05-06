namespace StellarApi.DTOs
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for a celestial object, including its properties.
    /// </summary>
    public class CelestialObjectDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the celestial object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the celestial object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the celestial object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the mass of the celestial object.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// Gets or sets the temperature of the celestial object.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Gets or sets the radius of the celestial object.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the image path of the celestial object.
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Gets or sets the position of the celestial object.
        /// </summary>
        public PositionDTO? Position { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the celestial object.
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the celestial object.
        /// </summary>
        public DateTime? ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the celestial object.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the list of properties associated with the celestial object.
        /// </summary>
        public IEnumerable<PropertyDTO> Metadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectDTO"/> class.
        /// </summary>
        public CelestialObjectDTO()
        {
            Metadata = new List<PropertyDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectDTO"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <param name="name">The name of the celestial object.</param>
        /// <param name="description">The description of the celestial object.</param>
        /// <param name="mass">The mass of the celestial object.</param>
        /// <param name="temperature">The temperature of the celestial object.</param>
        /// <param name="radius">The radius of the celestial object.</param>
        /// <param name="image">The image path of the celestial object.</param>
        /// <param name="creationDate">The creation date of the celestial object.</param>
        /// <param name="modificationDate">The last modification date of the celestial object.</param>
        /// <param name="metadata">The list of inherited properties of the object.</param>
        public CelestialObjectDTO(int id, string name, string description, PositionDTO? position, double mass, double temperature, double radius, 
            string image, DateTime creationDate, DateTime modificationDate, string type, IEnumerable<PropertyDTO> metadata)
        {
            Id = id;
            Name = name;
            Description = description;
            Position = position;
            Mass = mass;
            Temperature = temperature;
            Radius = radius;
            Image = image;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
            Type = type;
            Metadata = metadata;
        }
    }
}
