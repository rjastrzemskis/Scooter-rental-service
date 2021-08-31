using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Core.Exceptions;
using ScooterRental.Core.Modules;

namespace ScooterRentalTests
{
    [TestClass]
    public class AccountingTests : Exception
    {
        private readonly Accounting _accounting;
        private readonly IList<RentalData> _rentalDataList;

        public AccountingTests()
        {
            _rentalDataList = new List<RentalData>();
            _accounting = new Accounting(_rentalDataList);
        }

        [TestMethod]
        public void CalculateBill_CalculateBillForNullId_ScooterInvalidIdException()
        {
            //Arrange
            string id = null;
            DateTime startTime = DateTime.Now;
            _rentalDataList.Add(new RentalData
                { Id = id, PricePerMinute = 1, StarTime = startTime, EndTime = startTime.AddMinutes(10) });

            //Act
            Action act = () => _accounting.CalculateBill(id);

            //Assert
            Assert.ThrowsException<ScooterInvalidIdException>(act);
        }

        [TestMethod]
        public void CalculateBill_ScooterRented10Minutes_10Expected()
        {
            //Arrange
            string id = "2";
            DateTime startTime = DateTime.Now;
            _rentalDataList.Add(new RentalData
                { Id = id, PricePerMinute = 1, StarTime = startTime, EndTime = startTime.AddMinutes(10) });

            //Act
            decimal result = _accounting.CalculateBill(id);

            //Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void CalculateBill_TimeExceeds20Eur_20Expected()
        {
            //Arrange
            string id = "2";
            DateTime startTime = new DateTime(2021, 1, 1, 0, 0, 0);
            _rentalDataList.Add(new RentalData
                { Id = id, PricePerMinute = 10, StarTime = startTime, EndTime = startTime.AddHours(1) });

            //Act
            decimal result = _accounting.CalculateBill(id);

            //Assert
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void CalculateBill_TimeExceeds20EurPerDay_60Expected()
        {
            //Arrange
            string id = "2";
            DateTime startTime = new DateTime(2021, 1, 1, 0, 0, 0);
            _rentalDataList.Add(new RentalData
                { Id = id, PricePerMinute = 1, StarTime = startTime, EndTime = startTime.AddDays(3) });

            //Act
            decimal result = _accounting.CalculateBill(id);

            //Assert
            Assert.AreEqual(60, result);
        }

        [TestMethod]
        public void CalculateBill_WithMultipleDaysAnd5Minutes_65Expected()
        {
            //Arrange
            string id = "2";
            DateTime startTime = new DateTime(2021, 1, 1, 0, 0, 0);
            _rentalDataList.Add(new RentalData
                { Id = id, PricePerMinute = 1, StarTime = startTime, EndTime = startTime.AddDays(3).AddMinutes(5) });

            //Act
            decimal result = _accounting.CalculateBill(id);

            //Assert
            Assert.AreEqual(65.0m, result);
        }
    }
}
