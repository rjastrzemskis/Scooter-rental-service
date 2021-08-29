using System.Collections.Generic;
using ScooterRental.Core.Exceptions;
using ScooterRental.Core.Interfaces;

namespace ScooterRental.Core.Modules
{
    public class ScooterService : IScooterService
    {
        private readonly List<Scooter> _scootersList;

        public ScooterService()
        {
            _scootersList = new List<Scooter>();
        }

        public void AddScooter(string id, decimal pricePerMinute)
        {
            Scooter scooter = GetScooterById(id);
            if (id == null)
                throw new ScooterInvalidIdException();

            if (pricePerMinute <= 0)
                throw new ScooterInvalidPriceException();

            if (scooter != null)
                throw new ScooterSameIdException();

            _scootersList.Add(new Scooter(id, pricePerMinute));
        }

        public void RemoveScooter(string id)
        {
            Scooter scooter = GetScooterById(id);
            if (scooter.IsRented)
                throw new ScooterIsRentedException();

            _scootersList.Remove(scooter);
        }

        public IList<Scooter> GetScooters()
        {
            return _scootersList;
        }

        public Scooter GetScooterById(string scooterId)
        {
            Scooter scooter = _scootersList.Find(x => x.Id == scooterId);
            return scooter;
        }
    }
}
