using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HostelManage.Models;
using HostelManage.Data;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace HostelManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/user/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromForm] UserRequest request)
        {
            // 1. Required field validations
            if (string.IsNullOrEmpty(request.Name) ||
                string.IsNullOrEmpty(request.Address) ||
                string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Required fields are missing.");
                return BadRequest(new { message = "All required fields must be provided." });
            }

            // 2. Check if email already exists
            var existingUser = await _context.User
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (existingUser != null)
            {
                _logger.LogWarning($"Email {request.Email} is already registered.");
                return BadRequest(new { message = "Email is already registered." });
            }

            // 3. Check if image is provided
            if (request.Image == null)
            {
                _logger.LogWarning("Image is missing.");
                return BadRequest(new { message = "User image is required." });
            }

            // 4. Convert image to byte array
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.Image.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            // 5. Create new User object (without plain‐text password)
            // 6. Hash the password
            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                Name = request.Name,
                Address = request.Address,
                Email = request.Email.ToLower(),
                Image = imageBytes
            };

            user.Password = passwordHasher.HashPassword(user, request.Password);

            // 7. Save to database
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving user: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while saving the user." });
            }
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin request)
        {
            // 1. Validate required fields
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Email or Password is missing.");
                return BadRequest(new { message = "Email and Password are required." });
            }

            // 2. Find the user by email
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (user == null)
            {
                _logger.LogWarning($"User with email {request.Email} not found.");
                return NotFound(new { message = "User not found." });
            }

            // 3. Verify the hashed password
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Invalid password for email {Email}.", request.Email);
                return Unauthorized(new { message = "Invalid password." });
            }

            // 4. Success — return minimal user info
            return Ok(new
            {
                UserID = user.UserID,
                Name = user.Name
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (user == null)
            {
                _logger.LogWarning("Reset password: user with email {Email} not found.", request.Email);
                return NotFound(new { message = "User not found." });
            }

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, request.NewPassword);

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}.", request.Email);
                return StatusCode(500, new { message = "An error occurred while resetting the password." });
            }
        }

        // GET: api/user/hostel-rooms/{hostelId}
        [HttpGet("hostel-rooms/{hostelId}")]
        public async Task<IActionResult> GetHostelRooms(int hostelId)
        {
            var rooms = await _context.HostelRoom
                .Where(r => r.HostelID == hostelId)
                .Select(r => new
                {
                    r.RoomID,
                    RoomImage1 = r.RoomImage1 != null ? Convert.ToBase64String(r.RoomImage1) : null,
                    RoomImage2 = r.RoomImage2 != null ? Convert.ToBase64String(r.RoomImage2) : null,
                    RoomImage3 = r.RoomImage3 != null ? Convert.ToBase64String(r.RoomImage3) : null,
                    RoomImage4 = r.RoomImage4 != null ? Convert.ToBase64String(r.RoomImage4) : null
                })
                .ToListAsync();

            if (rooms == null || !rooms.Any())
            {
                return NotFound(new { message = "No rooms found for the given hostel ID." });
            }

            return Ok(rooms);
        }

        // GET: api/user/hostels
        [HttpGet("hostels")]
        public async Task<IActionResult> GetHostelsForUsers()
        {
            var hostels = await _context.Hostel
                .Where(h => h.Status == true)
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
                    HostelImage = h.HostelImage != null ? Convert.ToBase64String(h.HostelImage) : null,
                    h.Status
                })
                .ToListAsync();

            if (hostels == null || !hostels.Any())
            {
                return NotFound(new { message = "No hostels available." });
            }

            return Ok(hostels);
        }
    }

    // Request model for creating a user
    public class UserRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }

    // Request model for user login
    public class UserLogin
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }
    }
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters.")]
        public string NewPassword { get; set; }
    }

}
