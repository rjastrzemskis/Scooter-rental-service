using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Core.Exceptions;
using ScooterRental.Core.Interfaces;
using ScooterRental.Core.Modules;

namespace ScooterRentalTests
{
    [TestClass]
    public class ScooterServiceTests : Exception
    {
        private readonly IScooterService service;

        public ScooterServiceTests()
        {
            service = new ScooterService();
        }

        [TestMethod]
        public void AddScoooter_AddIdNull_InvalidIdException()
        {
            //Arrange
            string id = null;
            decimal pricePerMinute = 4;

            //Act
            Action act = () => service.AddScooter(id, pricePerMinute);

            //Assert
            Assert.ThrowsException<ScooterInvalidIdException>(act);
        }

        [TestMethod]
        public void AddScoooter_AddNegativePrice_InvalidPriceException()
        {
            //Arrange
            string id = "2";
            decimal pricePerMinute = -4;

            //Act
            Action act = () => service.AddScooter(id, pricePerMinute);

            //Assert
            Assert.ThrowsException<ScooterInvalidPriceException>(act);
        }

        [TestMethod]
        public void AddScoooter_AddSameScooterTwice_SameScooterIdException()
        {
            //Arrange
            string id = "2";
            decimal pricePerMinute = 4;

            //Act
            service.AddScooter(id, pricePerMinute);
            Action act = () => service.AddScooter(id, pricePerMinute);

            //Assert
            Assert.ThrowsException<ScooterSameIdException>(act);
        }

        [TestMethod]
        public void AddScoooter_AddScooter_Expected1()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 4;

            //Act
            service.AddScooter(id, pricePerMinute);
            IList<Scooter> scootersList = service.GetScooters();
            int result = scootersList.Count;

            //Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void RemoveScooter_RemoveScooterWhichIsRented_ScooterIsInRentException()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 4;
            service.AddScooter(id, pricePerMinute);
            Scooter scooter = service.GetScooterById(id);
            scooter.IsRented = true;

            //Act
            Action act = () => service.RemoveScooter(id);

            //Assert
            Assert.ThrowsException<ScooterIsRentedException>(act);
        }

        [TestMethod]
        public void RemoveScooter_RemoveScooter_Expected0()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 4;
            service.AddScooter("1", pricePerMinute);

            //Act
            service.RemoveScooter(id);
            IList<Scooter> scootersList = service.GetScooters();
            int result = scootersList.Count;

            //Assert
            Assert.AreEqual(0, result);
        }
    }
}
