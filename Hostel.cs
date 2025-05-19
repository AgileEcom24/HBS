using System;
using System.ComponentModel.DataAnnotations;

namespace HostelManage.Models
{
    public class Hostel
    {
        [Key]
        public int HostelID { get; set; }

        [Required]
        [StringLength(100)]
        public string HostelName { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "Number of room types must be between 1 and 4.")]
        public int NumberOfRoomTypes { get; set; }

        [StringLength(50)]
        public string? RoomType1 { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RateType1 { get; set; }

        [StringLength(50)]
        public string? RoomType2 { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RateType2 { get; set; }

        [StringLength(50)]
        public string? RoomType3 { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RateType3 { get; set; }

        [StringLength(50)]
        public string? RoomType4 { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RateType4 { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        public byte[]? DocumentImage { get; set; }

        [StringLength(50)]
        public string? DocumentNumber { get; set; }

        public byte[]? HostelImage { get; set; }

        [Required]
        public bool Status { get; set; } = false; // Default to unverified

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now; // Default to current date
    }
}
