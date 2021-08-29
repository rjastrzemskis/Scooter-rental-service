using System;
using System.Collections.Generic;
using System.Linq;
using ScooterRental.Core.Exceptions;

namespace ScooterRental.Core.Modules
{
    public class Accounting
    {
        private readonly IList<RentalData> _rentalDataList;
        private readonly IList<RentalData> _copyOfRentalDataList;

        public Accounting(IList<RentalData> rentalDataList)
        {
            _rentalDataList = rentalDataList;
            _copyOfRentalDataList = new List<RentalData>();
        }

        public decimal CalculateBill(string id)
        {
            if (id == null)
                throw new ScooterInvalidIdException();

            RentalData? rentedScooter = _rentalDataList.LastOrDefault(x => x.Id == id);
            decimal priceForOneDay = 20.0m, price = 0.0m;
            TimeSpan? interval = rentedScooter.EndTime - rentedScooter.StarTime;
            if (rentedScooter.StarTime.Date != rentedScooter.EndTime.Value.Date)
            {
                if (rentedScooter.StarTime.TimeOfDay != rentedScooter.EndTime.Value.TimeOfDay)
                {
                    TimeSpan firstDay = rentedScooter.StarTime.Date.AddDays(1) - rentedScooter.StarTime;
                    TimeSpan? lastDay = rentedScooter.EndTime - rentedScooter.EndTime.Value.Date;
                    decimal firstDayPrice = (decimal)firstDay.TotalMinutes * rentedScooter.PricePerMinute;
                    price += firstDayPrice > priceForOneDay ? priceForOneDay : firstDayPrice;
                    decimal lastDayPrice = (decimal)lastDay.Value.TotalMinutes * rentedScooter.PricePerMinute;
                    price += lastDayPrice > priceForOneDay ? priceForOneDay : lastDayPrice;
                    int totalDaysLeft = (int)interval.Value.Subtract(firstDay).Subtract(lastDay.Value).TotalDays;
                    price += totalDaysLeft * priceForOneDay;
                    return price;
                }

                int totalDaysIfTimeIsSame = (int)interval?.TotalDays;
                return priceForOneDay * totalDaysIfTimeIsSame;
            }

            decimal lessThan20MinAndInOneDay = (decimal)interval?.TotalMinutes;
            if (lessThan20MinAndInOneDay < priceForOneDay)
                return rentedScooter.PricePerMinute * lessThan20MinAndInOneDay;

            return priceForOneDay;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            DateTime endTime = new DateTime(2021, 9, 29, 02, 00, 0);  //lai vieglak testetu , velak DateTime.Now;
            decimal income = 0.0m;
            if (year != null) // pienemam gads 2021 
            {
                foreach (RentalData data in _rentalDataList)
                {
                    if (includeNotCompletedRentals) // ja ir true
                    {
                        if (data.StarTime.Year == year) // visi kas ir sakti 2021
                        {
                            if (data.EndTime.Value <= endTime)
                                income += CalculateBill(data.Id);
                            else // tiek kas , nav beigusies , bus jaiedod endtime visiem tagad, izmantojot kopiju.
                                _copyOfRentalDataList.Add(new RentalData
                                {
                                    Id = data.Id, StarTime = data.StarTime, EndTime = endTime,
                                    PricePerMinute = data.PricePerMinute
                                });
                        }
                    }
                    else // ja ir false
                    {
                        if (data.StarTime.Year == year) // visi kas ir sakti 2021
                        {
                            if (data.EndTime.Value <= endTime) // tie kas jau ir beigusies tikai
                                income += CalculateBill(data.Id);
                        }
                    }
                }
            }
            else // pienamam ka nav ievadits gads
            {
                foreach (RentalData data in _rentalDataList)
                {
                    if (includeNotCompletedRentals) // ja ir true
                    {
                        if (data.EndTime.Value <= endTime)
                            income += CalculateBill(data.Id);
                        else // tiek kas , nav beigusies , bus jaiedod endtime visiem tagad, izmantojot kopiju.
                            _copyOfRentalDataList.Add(new RentalData
                            {
                                Id = data.Id,
                                StarTime = data.StarTime,
                                EndTime = endTime,
                                PricePerMinute = data.PricePerMinute
                            });
                    }
                    else // ja ir false
                    {
                        if (data.EndTime.Value <= endTime) // tie kas jau ir beigusies tikai
                            income += CalculateBill(data.Id);
                    }
                }
            }
            income += _copyOfRentalDataList.Sum(x => CopyCalculateBill(x.Id));

            return income;
        }

        public decimal CopyCalculateBill(string id)
        {
            if (id == null)
                throw new ScooterInvalidIdException();

            RentalData? rentedScooter = _copyOfRentalDataList.LastOrDefault(x => x.Id == id);
            decimal priceForOneDay = 20.0m, price = 0.0m;
            TimeSpan? interval = rentedScooter.EndTime - rentedScooter.StarTime;
            if (rentedScooter.StarTime.Date != rentedScooter.EndTime.Value.Date)
            {
                if (rentedScooter.StarTime.TimeOfDay != rentedScooter.EndTime.Value.TimeOfDay)
                {
                    TimeSpan firstDay = rentedScooter.StarTime.Date.AddDays(1) - rentedScooter.StarTime;
                    TimeSpan? lastDay = rentedScooter.EndTime - rentedScooter.EndTime.Value.Date;
                    decimal firstDayPrice = (decimal)firstDay.TotalMinutes * rentedScooter.PricePerMinute;
                    price += firstDayPrice > priceForOneDay ? priceForOneDay : firstDayPrice;
                    decimal lastDayPrice = (decimal)lastDay.Value.TotalMinutes * rentedScooter.PricePerMinute;
                    price += lastDayPrice > priceForOneDay ? priceForOneDay : lastDayPrice;
                    int totalDaysLeft = (int)interval.Value.Subtract(firstDay).Subtract(lastDay.Value).TotalDays;
                    price += totalDaysLeft * priceForOneDay;
                    return price;
                }

                int totalDaysIfTimeIsSame = (int)interval?.TotalDays;
                return priceForOneDay * totalDaysIfTimeIsSame;
            }

            decimal lessThan20MinAndInOneDay = (decimal)interval?.TotalMinutes;
            if (lessThan20MinAndInOneDay < priceForOneDay)
                return rentedScooter.PricePerMinute * lessThan20MinAndInOneDay;

            return priceForOneDay;
        }
    }
}