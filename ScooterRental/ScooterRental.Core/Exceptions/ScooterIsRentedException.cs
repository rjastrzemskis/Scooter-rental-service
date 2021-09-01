using System;

namespace ScooterRental.Core.Exceptions
{
    public class ScooterIsRentedException : Exception
    {
        public ScooterIsRentedException() : base("Scooter with this Id is rented!")
        {

        }

        public ScooterIsRentedException(string message) : base(message)
        {

        }
    }
}
