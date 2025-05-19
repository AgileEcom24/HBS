using Microsoft.AspNetCore.Mvc;
using HostelManage.Models;
using Microsoft.EntityFrameworkCore;
using HostelManage.Data;

namespace HostelManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Booking/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetBookingCount()
        {
            return await _context.Booking.CountAsync();
        }

        // 2. POST: api/Booking
        [HttpPost]
        public async Task<ActionResult<Booking>> AddBooking(Booking booking)
        {
            // Optional: Validate that CheckOut is later than CheckIn
            if (booking.CheckOut <= booking.CheckIn)
            {
                return BadRequest("Check-Out must be after Check-In.");
            }

            booking.CreationDate = DateTime.Now;
            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingID }, booking);
        }

        // 3. GET: api/Booking/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUserId(int userId)
        {
            var bookings = await _context.Booking
                .Where(b => b.UserID == userId)
                .ToListAsync();

            return Ok(bookings);
        }

        // 4. GET: api/Booking/hostel/{hostelId}
        [HttpGet("hostel/{hostelId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByHostelId(int hostelId)
        {
            var bookings = await _context.Booking
                .Where(b => b.HostelID == hostelId)
                .ToListAsync();

            return Ok(bookings);
        }

        // 5. PUT: api/Booking/status/{id}?status=1
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromQuery] int status)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
                return NotFound();

            if (status < 0 || status > 2)
                return BadRequest("Invalid status value. Use 0 = Pending, 1 = Confirmed, 2 = Cancelled");

            booking.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 6. DELETE: api/Booking/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
                return NotFound();

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper: GET Booking by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
                return NotFound();

            return booking;
        }

        // GET: api/Booking For admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            return await _context.Booking.ToListAsync();
        }


    }
}
