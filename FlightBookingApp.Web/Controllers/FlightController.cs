using FlightBookingApp.Web.Data;
using FlightBookingApp.Web.Services;
using FlightBookingApp.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlightBookingApp.Web.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApp.Web.Controllers
{
    [Authorize]
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly PaymentService _paymentService;

        public FlightController(ApplicationDbContext context, NotificationService notificationService, PaymentService paymentService)
        {
            _context = context;
            _notificationService = notificationService;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var flights = await _context.Flights
                .Where(f => f.DepartureTime > now.AddHours(1))
                .ToListAsync();
            return View(flights);
        }

        [HttpGet]
        public IActionResult Book(int id)
        {
            var flight = _context.Flights.Find(id);
            return View(new BookingViewModel { Flight = flight });
        }

        [HttpPost]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var flight = await _context.Flights.FindAsync(model.Flight.Id);
                if (flight == null)
                {
                    ModelState.AddModelError("", "Flight not found.");
                    return View(model);
                }

                // Validate the expiry date
                if (!DateTime.TryParseExact(model.PaymentDetails.ExpiryDate, "MM/yy", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
                {
                    ModelState.AddModelError("PaymentDetails.ExpiryDate", "Invalid date format. Please use MM/yy.");
                    return View(model);
                }

                if (expiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("PaymentDetails.ExpiryDate", "The expiry date must be greater than or equal to today.");
                    return View(model);
                }

                // Get the current user's ID and name
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(model);
                }

                // Payment processing
                var paymentResult = await _paymentService.ProcessPayment(model.PaymentDetails);
                if (paymentResult.IsSuccess)
                {
                    var booking = new Booking
                    {
                        FlightId = model.Flight.Id,
                        UserId = userId,
                        UserName = userName,
                        CardLastFourDigits = model.PaymentDetails.CardNumber[^4..], // Last 4 digits of card
                        BookingDate = DateTime.Now
                    };
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    // Notification
                    await _notificationService.SendNotification(userId, "Your flight is booked!");

                    return RedirectToAction("Confirmation", new { id = booking.Id });
                }
                ModelState.AddModelError("", "Payment failed. Please try again.");
            }
            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Airline,Destination,DepartureTime")] Flight flight)
        {
            if (flight.DepartureTime <= DateTime.Now.AddDays(1))
            {
                ModelState.AddModelError("DepartureTime", "The departure time must be at least one day in the future.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Airline,Destination,DepartureTime")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (flight.DepartureTime <= DateTime.Now.AddDays(1))
            {
                ModelState.AddModelError("DepartureTime", "The departure time must be at least one day in the future.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
