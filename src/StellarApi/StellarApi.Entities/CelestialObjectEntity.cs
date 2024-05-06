using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StellarApi.Entities
{
    /// <summary>
    /// Represents an entity for a celestial object in a database.
    /// </summary>
    [Table("CelestialObject")]
    public class CelestialObjectEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the celestial object.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the celestial object.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The type cannot be empty.")]
        [MaxLength(20, ErrorMessage = "The type should be less than 20 caracters.")]
        public required string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the celestial object.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The name cannot be empty.")]
        [MaxLength(50, ErrorMessage = "The name should be less than 50 caracters.")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the celestial object.
        /// </summary>
        [MaxLength(1000, ErrorMessage = "The description should be less than 1000 caracters.")]
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the position of the celestial object.
        /// </summary>
        public required PositionEntity Position { get; set; } = new();

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
        /// Gets or sets the creation date of the celestial object.
        /// </summary>
        public required DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the celestial object.
        /// </summary>
        public required DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a planet has water.
        /// </summary>
        public bool? IsWater { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a planet supports life.
        /// </summary>
        public bool? IsLife { get; set; }

        /// <summary>
        /// Gets or sets the type of a planet.
        /// </summary>
        public string? PlanetType { get; set; }

        /// <summary>
        /// Gets or sets the brightness of a star.
        /// </summary>
        public double? Brightness { get; set; }

        /// <summary>
        /// Gets or sets the type of a star.
        /// </summary>
        public string? StarType { get; set; }
    }
}