using FlightBookingApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingApp.Web.ViewModel
{
    public class BookingViewModel
    {
        public Flight Flight { get; set; }

        [Required]
        public PaymentDetails PaymentDetails { get; set; }
    }
}
