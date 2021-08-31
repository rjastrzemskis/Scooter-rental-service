using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Core.Exceptions;
using ScooterRental.Core.Interfaces;
using ScooterRental.Core.Modules;

namespace ScooterRentalTests
{
    [TestClass]
    public class RentalCompanyTests : Exception
    {

        private readonly IScooterService _service;
        private readonly IRentalCompany _company;
        private readonly IList<RentalData> _rentalDataList;
        private readonly Accounting _accounting;
        private string Name => "Company";

        public RentalCompanyTests()
        {
            _service = new ScooterService();
            _rentalDataList = new List<RentalData>();
            _accounting = new Accounting(_rentalDataList);
            _company = new RentalCompany(Name, _service, _rentalDataList, _accounting);
        }

        public DateTime DateTimeHelper(string startTime)
        {
            DateTime dateTime = DateTime.Now;
            if ("startTime1" == startTime)
                dateTime = new DateTime(2021, 9, 29, 00, 00, 0);

            if ("startTime2" == startTime)
                dateTime = new DateTime(2021, 9, 28, 23, 45, 0);

            if ("startTime3" == startTime)
                dateTime = new DateTime(2020, 9, 29, 23, 45, 0);

            if ("startTime4" == startTime)
                dateTime = new DateTime(2021, 9, 25, 02, 00, 0);

            return dateTime;
        }

        [TestMethod]
        public void RentalCompany_NameProvided_SameNameExpected()
        {
            //Arrange
            RentalCompany result = new RentalCompany(Name, _service, _rentalDataList, _accounting);

            //Assert
            Assert.AreEqual(Name, result.Name);
        }

        [TestMethod]
        public void StartRent_StartRentForNullId_ScooterInvalidIdException()
        {
            //Arrange
            string id = null;

            //Act
            Action act = () => _company.StartRent(id);

            //Assert
            Assert.ThrowsException<ScooterInvalidIdException>(act);
        }

        [TestMethod]
        public void StartRent_StartRentForUsedScooter_ScooterIsInRentException()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 5;
            _service.AddScooter(id, pricePerMinute);
            _service.GetScooterById(id).IsRented = true;

            //Act
            Action act = () => _company.StartRent(id);

            //Assert
            Assert.ThrowsException<ScooterIsRentedException>(act);
        }

        [TestMethod]
        public void StartRent_StartRentForUnusedScooter_ExpectedTrue()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 5;
            _service.AddScooter(id, pricePerMinute);

            //Act
            _company.StartRent(id);
            bool result = _service.GetScooterById(id).IsRented;

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void StartRent_StartRentForExistingScooter_RentalHistoryRecordAdded()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 5;
            _service.AddScooter(id, pricePerMinute);

            //Act
            _company.StartRent(id);

            //Assert
            Assert.AreEqual(1.0m, _rentalDataList.Count);
        }

        [TestMethod]
        public void EndRent_StartRentForNullId_ScooterInvalidIdException()
        {
            //Arrange
            string id = null;

            //Act
            Action act = () => _company.EndRent(id);

            //Assert
            Assert.ThrowsException<ScooterInvalidIdException>(act);
        }

        [TestMethod]
        public void EndRent_EndRentForUnusedScooter_ScooterWasNotRented()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 5;
            _service.AddScooter(id, pricePerMinute);
            _service.GetScooterById(id).IsRented = false;

            //Act
            Action act = () => _company.EndRent(id);

            //Assert
            Assert.ThrowsException<ScooterNotRentedRented>(act);
        }

        [TestMethod]
        public void EndRent_EndRentForUsedScooter_ScooterWasNotRented()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 5;
            _service.AddScooter(id, pricePerMinute);
            _company.StartRent(id);

            //Act
            _company.EndRent(id);
            bool result = _service.GetScooterById(id).IsRented;

            //Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void EndRent_RentedScooter_HistoryRecordModified()
        {
            //Arrange
            string id = "1";
            decimal pricePerMinute = 5;
            _service.AddScooter(id, pricePerMinute);
            _company.StartRent(id);

            //Act
            _company.EndRent(id);
            RentalData result = _rentalDataList.FirstOrDefault(s => s.Id == id);

            //Assert
            Assert.IsNotNull(result?.EndTime);
        }

        [TestMethod]
        public void CalculateIncome_CalculateIncomeFor2021YearIncludeAllServices_Expected115()
        {
            //Arrange
            DateTime startTime1 = DateTimeHelper("startTime1");
            _rentalDataList.Add(new RentalData
                { Id = "1", PricePerMinute = 1, StarTime = startTime1, EndTime = startTime1.AddDays(1) });

            DateTime startTime2 = DateTimeHelper("startTime2");
            _rentalDataList.Add(new RentalData
                { Id = "2", PricePerMinute = 1, StarTime = startTime2, EndTime = startTime2.AddHours(5) });

            DateTime startTime3 = DateTimeHelper("startTime3");
            _rentalDataList.Add(new RentalData
                { Id = "3", PricePerMinute = 1, StarTime = startTime3, EndTime = startTime3.AddMinutes(10) });

            DateTime startTime4 = DateTimeHelper("startTime4");
            _rentalDataList.Add(new RentalData
                { Id = "4", PricePerMinute = 1, StarTime = startTime4, EndTime = startTime4.AddDays(3) });

            //Act
            decimal result = _company.CalculateIncome(2021, true);

            //Assert
            Assert.AreEqual(115.0m, result);
        }

        [TestMethod]
        public void CalculateIncome_CalculateIncomeFor2021YearNotIncludeAllServices_Expected60()
        {
            //Arrange
            DateTime startTime1 = DateTimeHelper("startTime1");
            _rentalDataList.Add(new RentalData
                { Id = "1", PricePerMinute = 1, StarTime = startTime1, EndTime = startTime1.AddDays(1) });

            DateTime startTime2 = DateTimeHelper("startTime2");
            _rentalDataList.Add(new RentalData
                { Id = "2", PricePerMinute = 1, StarTime = startTime2, EndTime = startTime2.AddHours(5) });

            DateTime startTime3 = DateTimeHelper("startTime3");
            _rentalDataList.Add(new RentalData
                { Id = "3", PricePerMinute = 1, StarTime = startTime3, EndTime = startTime3.AddMinutes(10) });

            DateTime startTime4 = DateTimeHelper("startTime4");
            _rentalDataList.Add(new RentalData
                { Id = "4", PricePerMinute = 1, StarTime = startTime4, EndTime = startTime4.AddDays(3) });

            //Act
            decimal result = _company.CalculateIncome(2021, false);

            //Assert
            Assert.AreEqual(60.0m, result);
        }

        [TestMethod]
        public void CalculateIncome_CalculateIncomeForNullYearIncludeAllServices_Expected125()
        {
            //Arrange
            DateTime startTime1 = DateTimeHelper("startTime1");
            _rentalDataList.Add(new RentalData
                { Id = "1", PricePerMinute = 1, StarTime = startTime1, EndTime = startTime1.AddDays(1) });

            DateTime startTime2 = DateTimeHelper("startTime2");
            _rentalDataList.Add(new RentalData
                { Id = "2", PricePerMinute = 1, StarTime = startTime2, EndTime = startTime2.AddHours(5) });

            DateTime startTime3 = DateTimeHelper("startTime3");
            _rentalDataList.Add(new RentalData
                { Id = "3", PricePerMinute = 1, StarTime = startTime3, EndTime = startTime3.AddMinutes(10) });

            DateTime startTime4 = DateTimeHelper("startTime4");
            _rentalDataList.Add(new RentalData
                { Id = "4", PricePerMinute = 1, StarTime = startTime4, EndTime = startTime4.AddDays(3) });

            //Act
            decimal result = _company.CalculateIncome(null, true);

            //Assert
            Assert.AreEqual(125.0m, result);
        }

        [TestMethod]
        public void CalculateIncome_CalculateIncomeForNullYearNotIncludeAllServices_Expected70()
        {
            //Arrange
            DateTime startTime1 = DateTimeHelper("startTime1");
            _rentalDataList.Add(new RentalData
                { Id = "1", PricePerMinute = 1, StarTime = startTime1, EndTime = startTime1.AddDays(1) });

            DateTime startTime2 = DateTimeHelper("startTime2");
            _rentalDataList.Add(new RentalData
                { Id = "2", PricePerMinute = 1, StarTime = startTime2, EndTime = startTime2.AddHours(5) });

            DateTime startTime3 = DateTimeHelper("startTime3");
            _rentalDataList.Add(new RentalData
                { Id = "3", PricePerMinute = 1, StarTime = startTime3, EndTime = startTime3.AddMinutes(10) });

            DateTime startTime4 = DateTimeHelper("startTime4");
            _rentalDataList.Add(new RentalData
                { Id = "4", PricePerMinute = 1, StarTime = startTime4, EndTime = startTime4.AddDays(3) });

            //Act
            decimal result = _company.CalculateIncome(null, false);

            //Assert
            Assert.AreEqual(70.0m, result);
        }

        [TestMethod]
        public void CalculateIncome_CalculateIncomeForNullYearIncludeAllServicesForSameIdMultiple_Expected70()
        {
            //Arrange
            DateTime startTime1 = DateTimeHelper("startTime1");
            _rentalDataList.Add(new RentalData
                { Id = "1", PricePerMinute = 1, StarTime = startTime1, EndTime = startTime1.AddDays(1) });

            DateTime startTime2 = DateTimeHelper("startTime2");
            _rentalDataList.Add(new RentalData
                { Id = "2", PricePerMinute = 1, StarTime = startTime2, EndTime = startTime2.AddHours(5) });

            DateTime startTime3 = DateTimeHelper("startTime3");
            _rentalDataList.Add(new RentalData
                { Id = "3", PricePerMinute = 1, StarTime = startTime3, EndTime = startTime3.AddMinutes(10) });

            DateTime startTime4 = DateTimeHelper("startTime4");
            _rentalDataList.Add(new RentalData
                { Id = "4", PricePerMinute = 1, StarTime = startTime4, EndTime = startTime4.AddDays(3) });

            //Act
            decimal result = _company.CalculateIncome(null, false);

            //Assert
            Assert.AreEqual(70.0m, result);
        }
    }
}
