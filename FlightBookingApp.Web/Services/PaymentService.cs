using FlightBookingApp.Web.Models;

namespace FlightBookingApp.Web.Services
{
    public class PaymentService
    {
        public async Task<PaymentResult> ProcessPayment(PaymentDetails paymentDetails)
        {
            // Mocking a payment processing service
            await Task.Delay(500);
            return new PaymentResult { IsSuccess = true };
        }
    }
}
