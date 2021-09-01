using System;

namespace ScooterRental.Core.Exceptions
{
    public class ScooterInvalidIdException : Exception
    {
        public ScooterInvalidIdException() : base("Invalid Id!")
        {

        }

        public ScooterInvalidIdException(string message) : base (message)
        {

        }
    }
}
