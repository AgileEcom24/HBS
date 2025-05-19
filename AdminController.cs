using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HostelManage.Models;
using HostelManage.Data;  // Import the namespace where ApplicationDbContext is located
using System.Threading.Tasks;

namespace HostelManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Inject ApplicationDbContext into the controller
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/admin/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Admin loginRequest)
        {
            // Validate input
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Username or password cannot be empty." });
            }
            var admin = await _context.Admin
                .FirstOrDefaultAsync(a => a.Username == loginRequest.Username && a.Password == loginRequest.Password);

            // If admin is not found, return Unauthorized
            if (admin == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // If login is successful, return OK
            return Ok(new { message = "Login successful!" });
        }
        [HttpGet("hostels/count")]
        public async Task<IActionResult> GetHostelCount()
        {
            var hostelCount = await _context.Hostel.CountAsync();

            return Ok(new { totalHostels = hostelCount });
        }
        [HttpGet("hostels")]
        public async Task<IActionResult> GetHostels()
        {
            var hostel = await _context.Hostel
                .Select(h => new
                {
                    h.HostelID,
                    h.HostelName,
                    h.Address,
                    h.NumberOfRoomTypes,
                    h.RoomType1,
                    h.RateType1,
                    h.RoomType2,
                    h.RateType2,
                    h.RoomType3,
                    h.RateType3,
                    h.RoomType4,
                    h.RateType4,
                    h.Email,
                    DocumentImage = h.DocumentImage != null ? Convert.ToBase64String(h.DocumentImage) : null,
                    h.DocumentNumber,
                    HostelImage= h.HostelImage != null ? Convert.ToBase64String(h.HostelImage) : null,
                    h.Status,
                    h.CreationDate
                })
                .ToListAsync();

            if (hostel == null || !hostel.Any())
            {
                return NotFound(new { message = "No hostels found." });
            }

            return Ok(hostel);
        }
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateHostelStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var hostel = await _context.Hostel.FindAsync(id);

            if (hostel == null)
            {
                return NotFound(new { message = "Hostel not found." });
            }

            // No need for range check since the status is a boolean
            hostel.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Hostel status updated successfully.", hostelId = id, newStatus = request.Status });
        }
        [HttpGet("users/count")]
        public async Task<IActionResult> GetUserCount()
        {
            var userCount = await _context.User.CountAsync();

            return Ok(new { totalUsers = userCount });
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.User
                .Select(u => new
                {
                    u.UserID,
                    u.Name,
                    u.Address,
                    u.Email,
                    Image = u.Image != null ? Convert.ToBase64String(u.Image) : null 
                })
                .ToListAsync();

            if (users == null || !users.Any())
            {
                return NotFound(new { message = "No users found." });
            }

            return Ok(users);
        }
    }

    public class UpdateStatusRequest
    {
        public bool Status { get; set; } 
    }

}
