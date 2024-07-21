using System.ComponentModel.DataAnnotations;

namespace FlightBookingApp.Web.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        public string Airline { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }
    }
}
