using Microsoft.EntityFrameworkCore;
using StellarApi.Entities;

namespace StellarApi.Repository.Context
{
    /// <summary>
    /// Represents the database context for managing celestial objects with seeded data.
    /// </summary>
    public class SpaceDbContextSeed : SpaceDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDbContextSeed"/> class.
        /// </summary>
        public SpaceDbContextSeed() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDbContextSeed"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public SpaceDbContextSeed(DbContextOptions<SpaceDbContext> options) : base(options)
        {

        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var position1 = new PositionEntity { X = 493, Y = 224, Z = 359 };
            var position2 = new PositionEntity { X = 142, Y = 23, Z = 98 };
            var position3 = new PositionEntity { };
            /*modelBuilder.Entity<CelestialObjectEntity>().HasData(
                new CelestialObjectEntity
                {
                    Id = 1,
                    Name = "Doyle Larsen",
                    Description = "In sint laborum ex mollit labore esse dolore amet dolore magna occaecat magna. Qui fugiat ex laborum dolore aliqua labore eu voluptate. Reprehenderit laborum magna velit anim anim aliquip irure sint irure in in non fugiat. Et exercitation nisi cupidatat ad est. Nulla reprehenderit non amet ad. Reprehenderit ex labore amet esse quis occaecat enim ea dolor in. Magna enim commodo eu enim velit aliqua velit laborum nostrud ullamco Lorem nostrud.\r\n",
                    Mass = 3038.66,
                    Temperature = 6.275818152084682e+32,
                    Radius = 4144946377.8,
                    Image = "http://placehold.it/32x32",
                    Position = position1,
                    CreationDate = DateTime.Parse("2016-12-31T06:29:20 -01:00"),
                    ModificationDate = DateTime.Parse("2024-01-06T03:43:30 -01:00"),
                    Type = "Planet",
                    IsWater = true,
                    IsLife = false,
                    PlanetType = "Gas"
                },
                new CelestialObjectEntity
                {
                    Id = 2,
                    Name = "Maribel Nelson",
                    Description = "Consectetur ex do ad anim. Tempor Lorem sit non ad anim voluptate pariatur quis nulla exercitation adipisicing occaecat laborum. Officia reprehenderit nisi exercitation magna reprehenderit et proident ea sunt est nisi esse pariatur do. Minim reprehenderit fugiat eiusmod aliqua amet. Eiusmod esse aute laboris nulla aute tempor nisi ex sunt.\r\n",
                    Mass = 1674.2,
                    Temperature = 2.0325979313116613e+32,
                    Radius = 6454358073.4,
                    Image = "http://placehold.it/32x32",
                    Position = position2,
                    CreationDate = DateTime.Parse("2021-06-22T07:04:33 -02:00"),
                    ModificationDate = DateTime.Parse("2024-01-10T10:28:12 -01:00"),
                    Type = "Planet",
                    IsWater = false,
                    IsLife = true,
                    PlanetType = "Undefined"
                },
                new CelestialObjectEntity
                {
                    Id = 3,
                    Name = "Ella Zamora",
                    Description = "Ullamco ea excepteur duis dolor qui sunt labore ipsum cupidatat cillum nulla sint enim. Est nulla aliquip dolor et. Reprehenderit nostrud anim est minim in voluptate pariatur sint. Exercitation excepteur laborum incididunt incididunt eu qui id ea do. Ex quis exercitation sunt qui.\r\n",
                    Mass = 1050.46,
                    Radius = 9544335518.2,
                    Position = position3,
                    CreationDate = DateTime.Parse("2016-12-23T10:42:24 -01:00"),
                    ModificationDate = DateTime.Parse("2024-03-01T02:15:44 -01:00"),
                    Type = "Planet",
                    IsWater = false,
                    IsLife = false,
                    PlanetType = "Undefined"
                }
            );*/

            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Email = "stellar-api@example.com",
                    Username = "StellarApi",
                    Password = "admin",
                    Role = "Administrator",
                    CreationDate = DateTime.Parse("08/18/2018 07:22:16"),
                    ModificationDate = DateTime.Parse("08/18/2019 07:22:16"),
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
