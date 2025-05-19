using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using HostelManage.Models;
using HostelManage.Data;

namespace HostelManage.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostelRoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HostelRoomController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload/{hostelId}")]
        public async Task<IActionResult> UploadRoomImages(
            int hostelId,
            IFormFile? roomImage1,
            IFormFile? roomImage2,
            IFormFile? roomImage3,
            IFormFile? roomImage4)
        {
            var room = new HostelRoom
            {
                HostelID = hostelId,
                RoomImage1 = await ConvertToBytes(roomImage1),
                RoomImage2 = await ConvertToBytes(roomImage2),
                RoomImage3 = await ConvertToBytes(roomImage3),
                RoomImage4 = await ConvertToBytes(roomImage4)
            };

            _context.HostelRoom.Add(room);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Room added successfully", room.RoomID });
        }

        private async Task<byte[]?> ConvertToBytes(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        [HttpGet("by-hostel/{hostelId}")]
        public async Task<IActionResult> GetRoomsByHostelId(int hostelId)
        {
            var rooms = await _context.HostelRoom
                .Where(r => r.HostelID == hostelId)
                .ToListAsync();

            if (rooms == null || rooms.Count == 0)
                return NotFound(new { message = "No rooms found for this Hostel ID." });

            var roomDtos = rooms.Select(r => new
            {
                r.RoomID,
                r.HostelID,
                RoomImage1 = r.RoomImage1 != null ? Convert.ToBase64String(r.RoomImage1) : null,
                RoomImage2 = r.RoomImage2 != null ? Convert.ToBase64String(r.RoomImage2) : null,
                RoomImage3 = r.RoomImage3 != null ? Convert.ToBase64String(r.RoomImage3) : null,
                RoomImage4 = r.RoomImage4 != null ? Convert.ToBase64String(r.RoomImage4) : null
            });

            return Ok(roomDtos);
        }
        [HttpDelete("delete/{roomId}")]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            var room = await _context.HostelRoom.FindAsync(roomId);

            if (room == null)
            {
                return NotFound(new { message = $"Room with ID {roomId} not found." });
            }

            _context.HostelRoom.Remove(room);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Room with ID {roomId} deleted successfully." });
        }


        [HttpPut("update-images/all/by-hostel/{hostelId}")]
        public async Task<IActionResult> UpdateAllRoomImagesByHostelId(
        int hostelId,
        IFormFile? roomImage1,
        IFormFile? roomImage2,
        IFormFile? roomImage3,
        IFormFile? roomImage4)
        {
            var rooms = await _context.HostelRoom
                .Where(r => r.HostelID == hostelId)
                .ToListAsync();

            if (rooms == null || rooms.Count == 0)
                return NotFound(new { message = "No rooms found for this Hostel ID." });

            foreach (var room in rooms)
            {
                if (roomImage1 != null) room.RoomImage1 = await ConvertToBytes(roomImage1);
                if (roomImage2 != null) room.RoomImage2 = await ConvertToBytes(roomImage2);
                if (roomImage3 != null) room.RoomImage3 = await ConvertToBytes(roomImage3);
                if (roomImage4 != null) room.RoomImage4 = await ConvertToBytes(roomImage4);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Images updated for all rooms under Hostel ID " + hostelId });
        }

    }
}
