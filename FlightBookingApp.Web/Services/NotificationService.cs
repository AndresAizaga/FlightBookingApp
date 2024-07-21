namespace FlightBookingApp.Web.Services
{
    public class NotificationService
    {
        public async Task SendNotification(string userId, string message)
        {
            // Mocking a notification service
            await Task.Delay(500);
            Console.WriteLine($"Notification sent to {userId}: {message}");
        }
    }
}
