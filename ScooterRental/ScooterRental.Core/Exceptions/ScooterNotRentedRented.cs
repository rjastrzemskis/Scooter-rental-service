using System;

namespace ScooterRental.Core.Exceptions
{
   public class ScooterNotRentedRented : Exception
    {
        public ScooterNotRentedRented() : base ("Scooter with this id was not rented!")
        {
            
        }

        public ScooterNotRentedRented(string message) : base (message)
        {
            
        }
    }
}
