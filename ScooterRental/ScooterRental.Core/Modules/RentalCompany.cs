using System;
using System.Collections.Generic;
using System.Linq;
using ScooterRental.Core.Exceptions;
using ScooterRental.Core.Interfaces;

namespace ScooterRental.Core.Modules
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IScooterService _service;
        private readonly IList<RentalData> _rentalDataList;
        private readonly Accounting _accounting;

        public RentalCompany(string name, IScooterService service, IList<RentalData> rentalDataList , Accounting accounting)
        {
            Name = name;
            _service = service;
            _rentalDataList = rentalDataList;
            _accounting = accounting;
        }

        public string Name { get; }

        public void StartRent(string id)
        {
            Scooter scooter = _service.GetScooterById(id);
            if (id == null)
                throw new ScooterInvalidIdException();

            if (scooter.IsRented)
                throw new ScooterIsRentedException();

            _rentalDataList.Add(new RentalData
                { Id = id, StarTime = DateTime.Now, EndTime = null, PricePerMinute = scooter.PricePerMinute });
            scooter.IsRented = true;
        }

        public decimal EndRent(string id)
        {
            Scooter scooter = _service.GetScooterById(id);
            if (id == null)
                throw new ScooterInvalidIdException();

            if (!scooter.IsRented)
                throw new ScooterNotRentedRented();

            RentalData record = _rentalDataList.FirstOrDefault(x => x.Id == id);
            record.EndTime = DateTime.Now;
            scooter.IsRented = false;

            return _accounting.CalculateBill(id);
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            return _accounting.CalculateIncome(year , includeNotCompletedRentals);
        }
    }
}
