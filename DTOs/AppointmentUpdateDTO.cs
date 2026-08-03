using System.ComponentModel.DataAnnotations;

namespace EpicMock.DTOs
{
    public class AppointmentUpdateDTO
    {
        [Required]
        public int ProviderId { get; set; }

        public int? RoomId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }
    }
}
