namespace StellarApi.DTOs.Space
{
    /// <summary>
    /// Represents a Space Image Data Transfer Object.
    /// </summary>
    public class SpaceImage
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
        /// Creates a new instance of the <see cref="SpaceImage"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the space image.</param>
        /// <param name="title">The title of the space image.</param>
        /// <param name="description">The description of the space image.</param>
        /// <param name="image">The image url of the space image.</param>
        /// <param name="shootingDate">The shooting date of the space image.</param>
        public SpaceImage(int id, string title, string description, string image, DateTime shootingDate)
        {
            Id = id;
            Title = title;
            Description = description;
            Image = image;
            ShootingDate = shootingDate;
        }
    }
}
