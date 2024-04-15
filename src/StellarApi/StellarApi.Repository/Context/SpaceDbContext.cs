using Microsoft.EntityFrameworkCore;
using StellarApi.Entities;
using StellarApi.Model.Space;

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
        public DbSet<CelestialObjectEntity> CelestialObjects { get; private set; }

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
        /// Configures the database connection and other options for the context.
        /// </summary>
        /// <param name="optionsBuilder">The builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=..\StellarApi.Repository\Database\SpaceDatabase.db");
            }
        }
    }
}
