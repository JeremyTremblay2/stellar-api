using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StellarApi.Entities
{
    /// <summary>
    /// Entity representing a Space Image.
    /// </summary>
    [Index(nameof(ShootingDate))]
    public class SpaceImageEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the space image.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the space image.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The title cannot be empty.")]
        [MaxLength(100, ErrorMessage = "The title should be less than 100 caracters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the space image.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The description cannot be empty.")]
        [MaxLength(3000, ErrorMessage = "The description should be less than 3000 caracters.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the image url of the space image.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The image URL cannot be empty.")]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the shooting date of the space image.
        /// </summary>
        [Required(ErrorMessage = "The shooting date cannot be empty.")]
        public DateTime ShootingDate { get; set; }
    }
}
