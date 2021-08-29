using System;

namespace ScooterRental.Core.Exceptions
{
    public class ScooterSameIdException : Exception
    {
        public ScooterSameIdException() : base ("Scooter with this Id already exists!")
        {

        }

        public ScooterSameIdException(string message) : base(message)
        {
            
        }
    }
}
