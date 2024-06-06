using StellarApi.DTOs.Geometry;
using StellarApi.Model.Space;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace StellarApi.DTOs.Space
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for a celestial object in input (less data than the complete object), including its properties.
    /// </summary>
    [SwaggerSchema("A celestial object in input, used to create or update a celestial object in the application.", ReadOnly = true)]
    public class CelestialObjectInput : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the unique identifier of the celestial object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The name of the celestial object.", Nullable = false)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The description of the celestial object.", Nullable = false)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the mass of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The mass of the celestial object.", Nullable = true)]
        public double Mass { get; set; }

        /// <summary>
        /// Gets or sets the temperature of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The temperature of the celestial object.", Nullable = true)]
        public double Temperature { get; set; }

        /// <summary>
        /// Gets or sets the radius of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The radius of the celestial object.", Nullable = true)]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the image path of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The image path of the celestial object.", Nullable = true)]
        public string? Image { get; set; }

        /// <summary>
        /// Gets or sets the position of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The position of the celestial object.", Nullable = true)]
        public Position? Position { get; set; }

        /// <summary>
        /// Gets or sets the type of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The type of the celestial object.", Nullable = false)]
        
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the planet type of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The type of the planet if the celestial object is a Planet.", Nullable = true)]
        public string? PlanetType { get; set; }

        /// <summary>
        /// Gets or sets the possibility of water on the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The possibility of water on the celestial object if the celestial object is a Planet.", Nullable = true)]
        public bool? IsWater { get; set; }

        /// <summary>
        /// Gets or sets the possibility of life on the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The possibility of life on the celestial object if the celestial object is a Planet.", Nullable = true)]
        public bool? IsLife { get; set; }

        /// <summary>
        /// Gets or set the star type of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The type of the star if the celestial object is a Star.", Nullable = true)]
        public string? StarType { get; set; }

        /// <summary>
        /// Gets or sets the brightness of the celestial object.
        /// </summary>
        [SwaggerSchema(Description = "The brightness of the celestial object if the celestial object is a Star.", Nullable = true)]
        public double? Brightness { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectInput"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <param name="name">The name of the celestial object.</param>
        /// <param name="description">The description of the celestial object.</param>
        /// <param name="mass">The mass of the celestial object.</param>
        /// <param name="temperature">The temperature of the celestial object.</param>
        /// <param name="radius">The radius of the celestial object.</param>
        /// <param name="image">The image path of the celestial object.</param>
        /// <param name="planetType">The type of the planet.</param>
        /// <param name="isWater">The possibility of water on the celestial object.</param>
        /// <param name="isLife">The possibility of life on the celestial object.</param>
        /// <param name="starType">The type of the star.</param>
        /// <param name="brightness">The brightness of the celestial object.</param>
        public CelestialObjectInput(int id, string name, string description, Position? position, double mass, double temperature, double radius,
            string image, string type, string? planetType = null, bool? isWater = null, bool? isLife = null, string? starType = null, double? brightness = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Position = position;
            Mass = mass;
            Temperature = temperature;
            Radius = radius;
            Image = image;
            Type = type;
            PlanetType = planetType;
            IsWater = isWater;
            IsLife = isLife;
            StarType = starType;
            Brightness = brightness;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.TryParse(Type, true, out CelestialObjectType celestialObjectType))
            {
                var enumValues = string.Join(", ", Enum.GetNames(typeof(CelestialObjectType)));
                yield return new ValidationResult($"Invalid value '{Type}' for the field '{nameof(Type)}'. Valid values are: {enumValues}.");
            }
            Type = celestialObjectType.ToString();

            if (PlanetType != null)
            {
                if (!Enum.TryParse(PlanetType, true, out PlanetType planetType))
                {
                    var enumValues = string.Join(", ", Enum.GetNames(typeof(PlanetType)));
                    yield return new ValidationResult($"Invalid value '{PlanetType}' for the field '{nameof(PlanetType)}'. Valid values are: {enumValues}.");
                }
                PlanetType = planetType.ToString();
            }
            if (StarType != null)
            {
                if (!Enum.TryParse(StarType, true, out StarType starType))
                {
                    var enumValues = string.Join(", ", Enum.GetNames(typeof(StarType)));
                    yield return new ValidationResult($"Invalid value '{StarType}' for the field '{nameof(StarType)}'. Valid values are: {enumValues}.");
                }
                StarType = starType.ToString();
            }
        }
    }
}
