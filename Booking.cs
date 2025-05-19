using System;
using System.ComponentModel.DataAnnotations;

namespace HostelManage.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        public int HostelID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        [Range(0, 2)]
        public int Status { get; set; }
    }
}
