using FlightBookingApp.Web.Data;
using FlightBookingApp.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlightBookingApp.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyFlights()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.Bookings
                .Include(b => b.Flight)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Reschedule(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || booking.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            var availableFlights = await _context.Flights
                .Where(f => f.DepartureTime > DateTime.Now)
                .ToListAsync();

            var viewModel = new RescheduleViewModel
            {
                Booking = booking,
                AvailableFlights = availableFlights
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reschedule(int bookingId, int newFlightId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null || booking.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            var newFlight = await _context.Flights.FindAsync(newFlightId);
            if (newFlight == null || newFlight.DepartureTime <= DateTime.Now)
            {
                ModelState.AddModelError("", "Invalid flight selected.");
                return RedirectToAction(nameof(Reschedule), new { id = bookingId });
            }

            booking.FlightId = newFlightId;
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyFlights));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null || booking.Flight.DepartureTime <= DateTime.Now)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyFlights));
        }
    }
}
