using System.ComponentModel.DataAnnotations;

namespace FlightBookingApp.Web.Models
{
    public class PaymentDetails
    {
        //There are 19 spaces in total due to the blanck spaces in the middle
        [Required]
        [StringLength(19, MinimumLength = 19, ErrorMessage = "Card number must be 16 digits.")]
        public string CardNumber { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits.")]
        public string CVV { get; set; }
    }
}
