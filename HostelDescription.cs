using System.ComponentModel.DataAnnotations;

namespace HostelManage.Models
{
    public class HostelDescription
    {
        [Key]
        public int DescriptionID { get; set; }

        [Required]
        public int HostelID { get; set; }

        [MaxLength(500)]
        public string Location { get; set; }

        public string Description { get; set; }

        public int RoomType1Count { get; set; } = 0;

        public int RoomType2Count { get; set; } = 0;

        public int RoomType3Count { get; set; } = 0;

        public int RoomType4Count { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
