using FlightBookingApp.Web.Data;
using FlightBookingApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApp.Web.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationService> _logger;

        public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await NotifyUsersAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task NotifyUsersAsync()
        {
            //Check every x hours if there are flights to notify
            _logger.LogWarning("Checking for flight notifications to be sent.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var flightsTomorrow = await context.Flights
                    .Where(f => f.DepartureTime.Date == DateTime.Now.Date.AddDays(1))
                    .ToListAsync();

                foreach (var flight in flightsTomorrow)
                {
                    var bookings = await context.Bookings
                        .Where(b => b.FlightId == flight.Id)
                        .ToListAsync();

                    foreach (var booking in bookings)
                    {
                        _logger.LogWarning($"Sending notification to {booking.UserName} for flight {flight.Airline} to {flight.Destination} on {flight.DepartureTime.ToString("dd/MM/yyy HH:mm")}");
                    }
                }
            }
        }
    }
}
