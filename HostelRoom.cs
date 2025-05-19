using System.ComponentModel.DataAnnotations;

namespace HostelManage.Models
{
    public class HostelRoom
    {
        [Key]
        public int RoomID { get; set; }

        public int HostelID { get; set; }

        public byte[]? RoomImage1 { get; set; }

        public byte[]? RoomImage2 { get; set; }

        public byte[]? RoomImage3 { get; set; }

        public byte[]? RoomImage4 { get; set; }
    }
}
