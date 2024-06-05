using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using StellarApi.Entities;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.Repository.Exceptions;
using StellarApi.Repository.Repositories;
using StellarApi.EntityToModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.ApiUnitTests.StellarApi.Repository.Repositories
{

    [TestClass]
    public class CelestialObjectRepositoryTests
    {
        private Mock<SpaceDbContext> _contextMock;
        private ICelestialObjectRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _contextMock = new Mock<SpaceDbContext>();
            _repository = new CelestialObjectRepository(_contextMock.Object);
        }

        [TestMethod]
        public async Task AddCelestialObject_WhenDatabaseIsAvailable_ShouldReturnTrue()
        {
            // Arrange
            var celestialObject = new Star(1, "Sun", "The Sun is the star at the center of the Solar System.", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);

            // Act
            var result = await _repository.AddCelestialObject(celestialObject);

            // Assert
            Assert.IsTrue(result);
            _contextMock.Verify(m => m.AddAsync(It.IsAny<CelestialObjectEntity>(), It.IsAny<CancellationToken>()), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public void AddCelestialObject_WhenDatabaseIsNotAvailable_ShouldThrowUnavailableDatabaseException()
        {
            // Arrange
            _contextMock.Setup(c => c.CelestialObjects).Returns((DbSet<CelestialObjectEntity>)null);
            var celestialObject = new Star(1, "Sun", "The Sun is the star at the center of the Solar System.", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);

            // Act & Assert
            Assert.ThrowsExceptionAsync<UnavailableDatabaseException>(async () => await _repository.AddCelestialObject(celestialObject));
        }

        [TestMethod]
        public async Task EditCelestialObject_WhenDatabaseIsAvailableAndObjectExists_ShouldReturnTrue()
        {
            // Arrange
            CelestialObject celestialObject = new Star(2, "Sun", "The Sun is the star at the center of the Solar System.", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);

            CelestialObject existingEntity = new Planet(2, "Earth", "The Earth is a planet with the majority of its surface covered by water in the Solar system.", "image",
                new Model.Geometry.Position(1, 20, 30), 5.9722 * Math.Pow(10, 24), 15, 6371, true, true, PlanetType.Terrestrial);

            var entity = existingEntity.ToEntity();

            _contextMock.Setup(c => c.CelestialObjects.FindAsync(2)).ReturnsAsync(entity);
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _repository.EditCelestialObject(1, celestialObject);

            // Assert
            Assert.IsTrue(result);
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
            Assert.AreEqual("Sun", existingEntity.Name);
        }

        [TestMethod]
        public async Task EditCelestialObject_WhenDatabaseIsNotAvailable_ShouldThrowUnavailableDatabaseException()
        {
            // Arrange
            _contextMock.Setup(c => c.CelestialObjects).Returns((DbSet<CelestialObjectEntity>)null);
            var celestialObject = new Star(1, "Sun", "Description", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnavailableDatabaseException>(() => _repository.EditCelestialObject(1, celestialObject));
        }

        [TestMethod]
        public async Task EditCelestialObject_WhenObjectDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var celestialObject = new Star(1, "Sun", "Description", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);

            _contextMock.Setup(c => c.CelestialObjects.FindAsync(1)).ReturnsAsync((CelestialObjectEntity)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<EntityNotFoundException>(() => _repository.EditCelestialObject(1, celestialObject));
        }

        [TestMethod]
        public async Task EditCelestialObject_WhenSaveChangesFails_ShouldReturnFalse()
        {
            // Arrange
            var celestialObject = new Star(1, "Sun", "Description", "image",
                new Model.Geometry.Position(0, 0, 0), 1.989 * Math.Pow(10, 30), 5778, 696340, 70000, StarType.YellowDwarf, DateTime.Now, DateTime.Now);

            CelestialObject existingEntity = new Planet(2, "Earth", "The Earth is a planet with the majority of its surface covered by water in the Solar system.", "image",
                new Model.Geometry.Position(1, 20, 30), 5.9722 * Math.Pow(10, 24), 15, 6371, true, true, PlanetType.Terrestrial);

            _contextMock.Setup(c => c.CelestialObjects.FindAsync(1)).ReturnsAsync(existingEntity.ToEntity());
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            // Act
            var result = await _repository.EditCelestialObject(1, celestialObject);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
