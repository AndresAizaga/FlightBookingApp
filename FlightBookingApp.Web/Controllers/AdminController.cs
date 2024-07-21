using FlightBookingApp.Web.Data;
using FlightBookingApp.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Flight)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> EditBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight) // Include the related Flight entity
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var availableFlights = await _context.Flights
                .Where(f => f.DepartureTime > DateTime.Now)
                .ToListAsync();

            var viewModel = new EditBookingViewModel
            {
                Booking = booking,
                AvailableFlights = availableFlights
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBooking(EditBookingViewModel viewModel)
        {
            if (viewModel.Booking.Id > 0 && viewModel.NewFlightId > 0)
            {
                var booking = await _context.Bookings
                    .Include(b => b.Flight) // Include the related Flight entity
                    .FirstOrDefaultAsync(b => b.Id == viewModel.Booking.Id);

                if (booking == null)
                {
                    return NotFound();
                }

                // Update the booking with the new flight ID
                booking.FlightId = viewModel.NewFlightId;

                // Fetch the new flight to update the Flight navigation property
                booking.Flight = await _context.Flights.FindAsync(viewModel.NewFlightId);

                _context.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }

            // If we got this far, something failed, redisplay form
            var availableFlights = await _context.Flights
                .Where(f => f.DepartureTime > DateTime.Now)
                .ToListAsync();

            viewModel.AvailableFlights = availableFlights;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
