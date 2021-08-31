using System;
using System.Collections.Generic;
using System.Linq;
using ScooterRental.Core.Exceptions;

namespace ScooterRental.Core.Modules
{
    public class Accounting
    {
        private readonly IList<RentalData> _rentalDataList;

        public Accounting(IList<RentalData> rentalDataList)
        {
            _rentalDataList = rentalDataList;
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

        public decimal IncomeCounting(DateTime starTime, DateTime? endTime, decimal pricePerMinute)
        {
            decimal priceForOneDay = 20.0m, price = 0.0m;
            TimeSpan? interval = endTime - starTime;
            if (starTime.Date != endTime.Value.Date)
            {
                if (starTime.TimeOfDay != endTime.Value.TimeOfDay)
                {
                    TimeSpan firstDay = starTime.Date.AddDays(1) - starTime;
                    TimeSpan? lastDay = endTime - endTime.Value.Date;
                    decimal firstDayPrice = (decimal)firstDay.TotalMinutes * pricePerMinute;
                    price += firstDayPrice > priceForOneDay ? priceForOneDay : firstDayPrice;
                    decimal lastDayPrice = (decimal)lastDay.Value.TotalMinutes * pricePerMinute;
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
                return pricePerMinute * lessThan20MinAndInOneDay;

            return priceForOneDay;
        }
    }
}
