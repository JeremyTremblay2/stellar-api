namespace StellarApi.Model.Space
{
    /// <summary>
    /// Represents a Space Image.
    /// </summary>
    public class SpaceImage : IEquatable<SpaceImage>, IComparable<SpaceImage>, IComparable
    {
        /// <summary>
        /// The unique identifier of the space image.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The title of the space image.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the space image.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The image url of the space image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The shooting date of the space image.
        /// </summary>
        public DateTime ShootingDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceImage"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the space image.</param>
        /// <param name="title">The title of the space image.</param>
        /// <param name="description">The description of the space image.</param>
        /// <param name="image">The image url of the space image.</param>
        /// <param name="shootingDate">The shooting date of the space image.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="title"/> or <paramref name="image"/> is null or empty.</exception>
        public SpaceImage(int id, string title, string description, string image, DateTime shootingDate)
        {
            Id = id;
            Title = title;
            Description = description;
            Image = image;
            ShootingDate = shootingDate;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(SpaceImage) && Equals((SpaceImage)obj);
        }

        /// <inheritdoc/>
        public bool Equals(SpaceImage? other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }

        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not SpaceImage image)
                throw new ArgumentException("Object is not a Space Image", nameof(obj));
            return CompareTo(image);
        }

        /// <inheritdoc/>
        public int CompareTo(SpaceImage? other)
        {
            return other == null ? 1 : string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}