using System;

namespace ScooterRental.Core.Exceptions
{
    public class ScooterInvalidPriceException : Exception
    {
        public ScooterInvalidPriceException() : base("Invalid price!")
        {
            
        }

        public ScooterInvalidPriceException(string message) : base(message)
        {
            
        }
    }
}
