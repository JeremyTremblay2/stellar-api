using Microsoft.EntityFrameworkCore;
using StellarApi.Entities;
using StellarApi.Model.Space;

namespace StellarApi.Repository.Context
{
    public class SpaceDbContext : DbContext
    {
        public DbSet<CelestialObject> CelestialObjects { get; private set; }

        public SpaceDbContext() { }

        public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=C:\Users\jerem\Documents\Cours\Webservices\stellar-api\src\StellarApi\StellarApi.Repository\Database\SpaceDatabase.db");
            }
        }
    }
}
