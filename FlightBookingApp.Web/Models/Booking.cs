using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingApp.Web.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int FlightId { get; set; }
        public Flight Flight { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public string CardLastFourDigits { get; set; }

        [Required]
        public string UserName { get; set; }

        [NotMapped]
        public FlightStatus Status
        {
            get
            {
                var now = DateTime.Now;
                if (Flight.DepartureTime < now)
                    return FlightStatus.Completed;
                else if (Flight.DepartureTime < now.AddHours(4))
                    return FlightStatus.Boarding;
                else
                    return FlightStatus.Pending;
            }
        }
    }
}
