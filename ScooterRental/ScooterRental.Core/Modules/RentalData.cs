using System;

namespace ScooterRental.Core.Modules
{
    public class RentalData
    {
        public string Id { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal PricePerMinute { get; set; }
    }
}
