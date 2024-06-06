using Swashbuckle.AspNetCore.Annotations;

namespace StellarApi.DTOs.Geometry
{
    /// <summary>
    /// Represents a position in a three-dimensional space.
    /// </summary>
    [SwaggerSchema("A position in a three-dimensional space, used to place a celestial object on a map.", ReadOnly = true)]
    public class Position
    {
        /// <summary>
        /// Gets or sets the X coordinate of the position.
        /// </summary>
        [SwaggerSchema(Description = "The X coordinate of the position.", Nullable = false)]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the position.
        /// </summary>
        [SwaggerSchema(Description = "The Y coordinate of the position.", Nullable = false)]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the Z coordinate of the position.
        /// </summary>
        [SwaggerSchema(Description = "The Z coordinate of the position.", Nullable = false)]
        public int Z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class with specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the position.</param>
        /// <param name="y">The Y coordinate of the position.</param>
        /// <param name="z">The Z coordinate of the position.</param>
        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
