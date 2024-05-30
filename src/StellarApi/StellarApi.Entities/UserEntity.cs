using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StellarApi.Entities
{
    /// <summary>
    /// Represents an entity for a user in a database.
    /// </summary>
    [Table("User")]
    [Index(nameof(Email), IsUnique = true)]
    public class UserEntity
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The email cannot be empty.")]
        [MaxLength(100, ErrorMessage = "The email should be less than 100 caracters.")]
        public required string Email { get; set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The username cannot be empty.")]
        [MaxLength(20, ErrorMessage = "The email should be less than 20 caracters.")]
        public required string Username { get; set; }

        /// <summary>
        /// Gets the password of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The password cannot be empty.")]
        public required string Password { get; set; }

        /// <summary>
        /// Gets the role of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The role cannot be empty.")]
        public required string Role { get; set; }

        /// <summary>
        /// Gets the creation date of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The creation date cannot be empty.")]
        public required DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets the modification date of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The modification date cannot be empty.")]
        public required DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets the refresh token of the user.
        /// </summary>
        public string? RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets the expiry time of the refresh token.
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
