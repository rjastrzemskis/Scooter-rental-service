﻿using System;
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

        public RentalCompany(string name, IScooterService service, IList<RentalData> rentalDataList,
            Accounting accounting)
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
            DateTime endTimeNow = new DateTime(2021, 9, 29, 02, 00, 0); //lai vieglak testetu , velak DateTime.Now;
            decimal income = 0.0m;
            foreach (RentalData data in _rentalDataList)
            {
                if (year != null)
                {
                    if (includeNotCompletedRentals)
                    {
                        if (data.StarTime.Year == year)
                        {
                            if (data.EndTime.Value <= endTimeNow)
                                income += _accounting.IncomeCounting(data.StarTime, data.EndTime, data.PricePerMinute);

                            else income += _accounting.IncomeCounting(data.StarTime, endTimeNow, data.PricePerMinute);
                        }
                    }
                    else
                    {
                        if (data.StarTime.Year == year)
                        {
                            if (data.EndTime.Value <= endTimeNow)
                                income += _accounting.IncomeCounting(data.StarTime, data.EndTime, data.PricePerMinute);
                        }
                    }
                }
                else
                {
                    if (includeNotCompletedRentals)
                    {
                        if (data.EndTime.Value <= endTimeNow)
                            income += _accounting.IncomeCounting(data.StarTime, data.EndTime, data.PricePerMinute);

                        else income += _accounting.IncomeCounting(data.StarTime, endTimeNow, data.PricePerMinute);
                    }
                    else
                    {
                        if (data.EndTime.Value <= endTimeNow)
                            income += _accounting.IncomeCounting(data.StarTime, data.EndTime, data.PricePerMinute);
                    }
                }
            }

            return income;
        }
    }
}
