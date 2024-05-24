using Microsoft.AspNetCore.Routing;
using StellarApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.ApiUnitTests
{
    [TestClass]
    public class DateHelpersTest
    {
        [TestMethod]
        public void CheckDates_ModificationDateInFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime creationDate = DateTime.Now;
            DateTime modificationDate = DateTime.Now.AddDays(1);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => DateHelper.CheckDates(creationDate, modificationDate));
        }

        [TestMethod]
        public void CheckDates_ModificationDateBeforeCreationDate_ThrowsArgumentException()
        {
            // Arrange
            DateTime creationDate = DateTime.Now;
            DateTime modificationDate = DateTime.Now.AddDays(-1);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => DateHelper.CheckDates(creationDate, modificationDate));
        }

        [TestMethod]
        public void CheckDates_CreationDateInFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime creationDate = DateTime.Now.AddDays(1);
            DateTime? modificationDate = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => DateHelper.CheckDates(creationDate, modificationDate));
        }

        [TestMethod]
        public void CheckDates_NoCreationDateAndNoModificationDate_ReturnsCurrentDates()
        {
            // Arrange
            DateTime? creationDate = null;
            DateTime? modificationDate = null;

            // Act
            var result = DateHelper.CheckDates(creationDate, modificationDate);

            // Assert
            Assert.AreEqual(DateTime.Now.Date, result.CreationDate.Date);
            Assert.AreEqual(DateTime.Now.Date, result.ModificationDate.Date);
        }

        [TestMethod]
        public void CheckDates_ValidCreationDateAndNoModificationDate_ReturnsCreationDateAndCurrentModificationDate()
        {
            // Arrange
            DateTime creationDate = DateTime.Now.AddDays(-1);
            DateTime? modificationDate = null;

            // Act
            var result = DateHelper.CheckDates(creationDate, modificationDate);

            // Assert
            Assert.AreEqual(creationDate.Date, result.CreationDate.Date);
            Assert.AreEqual(DateTime.Now.Date, result.ModificationDate.Date);
        }

        [TestMethod]
        public void CheckDates_ValidCreationDateAndValidModificationDate_ReturnsCreationDateAndModificationDate()
        {
            // Arrange
            DateTime creationDate = DateTime.Now.AddDays(-2);
            DateTime modificationDate = DateTime.Now.AddDays(-1);

            // Act
            var result = DateHelper.CheckDates(creationDate, modificationDate);

            // Assert
            Assert.AreEqual(creationDate.Date, result.CreationDate.Date);
            Assert.AreEqual(modificationDate.Date, result.ModificationDate.Date);
        }
    }
}
