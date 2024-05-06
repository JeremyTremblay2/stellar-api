namespace StellarApi.DTOs
{
    /// <summary>
    /// Represents a position in a three-dimensional space.
    /// </summary>
    public class PositionDTO
    {
        /// <summary>
        /// Gets or sets the X coordinate of the position.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the position.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the Z coordinate of the position.
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionDTO"/> class with specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the position.</param>
        /// <param name="y">The Y coordinate of the position.</param>
        /// <param name="z">The Z coordinate of the position.</param>
        public PositionDTO(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
