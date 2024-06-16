using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StellarApi.Entities;

/// <summary>
/// Represents an entity for a map in a database.
/// </summary>
[Table("Map")]
public class MapEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the map.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the map.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The name cannot be empty.")]
    [MaxLength(100, ErrorMessage = "The name should be less than 100 characters.")]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the celestial objects in the map.
    /// </summary>
    public ICollection<CelestialObjectEntity> CelestialObjects { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the map.
    /// </summary>
    public required DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the modification date of the map.
    /// </summary>
    public required DateTime ModificationDate { get; set; }

    /// <summary>
    /// Gets or sets the user author identifier of the map.
    /// </summary>
    public int UserAuthorId { get; set; }

    /// <summary>
    /// Gets or sets the user author of the map.
    /// </summary>
    [ForeignKey("UserAuthorId")]
    public UserEntity? UserAuthor { get; set; }

    /// <summary>
    /// Gets or sets the public status of the map.
    /// </summary>
    public bool IsPublic { get; set; }
}