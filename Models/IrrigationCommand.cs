
using System.ComponentModel.DataAnnotations;

namespace AgriStreamIoT.Models
{
    public class IrrigationCommand
    {
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; } = string.Empty;

        public string Command { get; set; } = "OFF";

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
