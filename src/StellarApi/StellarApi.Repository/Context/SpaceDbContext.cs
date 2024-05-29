using Microsoft.EntityFrameworkCore;
using StellarApi.Entities;

namespace StellarApi.Repository.Context
{
    /// <summary>
    /// Represents the database context for managing celestial objects.
    /// </summary>
    public class SpaceDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet of celestial objects.
        /// </summary>
        public DbSet<CelestialObjectEntity> CelestialObjects { get; protected set; }

        /// <summary>
        /// Gets or sets the DbSet of users.
        /// </summary>
        public DbSet<UserEntity> Users { get; protected set; }

        /// <summary>
        /// Gets or sets the DbSet of space images.
        /// </summary>
        public DbSet<SpaceImageEntity> SpaceImages { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDbContext"/> class.
        /// </summary>
        public SpaceDbContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        protected SpaceDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
