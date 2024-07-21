using FlightBookingApp.Web.Models;

namespace FlightBookingApp.Web.ViewModel
{
    public class RescheduleViewModel
    {
        public Booking Booking { get; set; }
        public IEnumerable<Flight> AvailableFlights { get; set; }
        public int NewFlightId { get; set; }
    }
}
