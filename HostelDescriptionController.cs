using HostelManage.Data;
using HostelManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HostelManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HostelDescriptionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HostelDescriptionController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/HostelDescription
        [HttpPost]
        public async Task<IActionResult> AddHostelDescription([FromBody] HostelDescription model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Optional: check if HostelID exists in Hostel table before inserting
            var hostelExists = await _context.Hostel.FindAsync(model.HostelID);
            if (hostelExists == null)
            {
                return NotFound($"Hostel with ID {model.HostelID} does not exist.");
            }

            _context.HostelDescription.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHostelDescriptionByHostelId), new { hostelId = model.HostelID }, model);
        }

        // GET method to retrieve a description by HostelID
        [HttpGet("by-hostel/{hostelId}")]
        public async Task<IActionResult> GetHostelDescriptionByHostelId(int hostelId)
        {
            var description = await _context.HostelDescription
                .FirstOrDefaultAsync(h => h.HostelID == hostelId);

            if (description == null)
                return NotFound($"No description found for HostelID {hostelId}.");

            return Ok(description);
        }
    }
}
