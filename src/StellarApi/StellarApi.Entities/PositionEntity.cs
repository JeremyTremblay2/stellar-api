using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StellarApi.Entities
{
    /// <summary>
    /// Represents an entity for a celestial object in a database.
    /// </summary>
    [ComplexType]
    public class PositionEntity
    {
        /// <summary>
        /// Gets or sets the X coordinate of the position.
        /// </summary>
        public int? X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the position.
        /// </summary>
        public int? Y { get; set; }

        /// <summary>
        /// Gets or sets the Z coordinate of the position.
        /// </summary>
        public int? Z { get; set; }
    }
}
