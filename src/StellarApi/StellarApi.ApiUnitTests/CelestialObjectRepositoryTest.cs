using Microsoft.EntityFrameworkCore;
using Moq;
using StellarApi.Entities;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.Repository.Repositories;

namespace StellarApi.ApiUnitTests
{
    [TestClass]
    public class CelestialObjectRepositoryTest
    {
        [TestMethod]
        public async Task AddCelestialObject_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            /*var celestialObject = new Star(1, "Sun", "The Sun is the star at the center of the Solar System.", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);
            var mockSet = new Mock<DbSet<CelestialObjectEntity>>();
            var mockContext = new Mock<SpaceDbContext>();*/

            var repository = new CelestialObjectRepository();

            // Act
            /*var result = await repository.AddCelestialObject(celestialObject);

            // Assert
            Assert.IsTrue(result);
            mockSet.Verify(m => m.AddAsync(It.IsAny<CelestialObjectEntity>(), It.IsAny<CancellationToken>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());*/
        }
    }
}
