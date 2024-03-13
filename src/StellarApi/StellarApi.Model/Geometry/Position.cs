using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.Model.Geometry
{
    /// <summary>
    /// Represents a position in 3D space.
    /// </summary>
    public class Position : IEquatable<Position?>, IComparable<Position>, IComparable
    {
        /// <summary>
        /// Gets the X coordinate of the position.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate of the position.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the Z coordinate of the position.
        /// </summary>
        public int Z { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Position class with specified X, Y, and Z coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Position)) return false;
            return Equals(obj as Position);
        }

        /// <inheritdoc/>
        public bool Equals(Position? other)
        {
            return other is not null &&
                   X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is Position otherPosition) return CompareTo(otherPosition);
            throw new ArgumentException("Object is not a Position");
        }

        /// <inheritdoc/>
        public int CompareTo(Position? other)
        {
            if (other == null) return 1;
            if (X != other.X) return X.CompareTo(other.X);
            if (Y != other.Y) return Y.CompareTo(other.Y);
            return Z.CompareTo(other.Z);
        }

        /// <inheritdoc/>
        public override string ToString() => $"({X}; {Y}; {Z})";

        /// <summary>
        /// Determines whether two Position objects are equal.
        /// </summary>
        public static bool operator ==(Position? left, Position? right)
        {
            return left != null && right != null && left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        /// <summary>
        /// Determines whether two Position objects are not equal.
        /// </summary>
        public static bool operator !=(Position? left, Position? right)
            => !(left == right);

        /// <summary>
        /// Determines whether the first Position is greater than the second Position.
        /// </summary>
        public static bool operator >(Position? left, Position? right)
        {
            if (left is null) return false;
            return right is null || left.X > right.X || (left.X == right.X && (left.Y > right.Y || (left.Y == right.Y && left.Z > right.Z)));
        }

        /// <summary>
        /// Determines whether the first Position is less than the second Position.
        /// </summary>
        public static bool operator <(Position? left, Position? right)
        {
            if (right is null) return false;
            return left is null || left.X < right.X || (left.X == right.X && (left.Y < right.Y || (left.Y == right.Y && left.Z < right.Z)));
        }

        /// <summary>
        /// Determines whether the first Position is greater than or equal to the second Position.
        /// </summary>
        public static bool operator >=(Position? left, Position? right)
            => left > right || left == right;

        /// <summary>
        /// Determines whether the first Position is less than or equal to the second Position.
        /// </summary>
        public static bool operator <=(Position? left, Position? right)
            => left < right || left == right;
    }
}