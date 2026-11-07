
using System.ComponentModel.DataAnnotations;

namespace AgriStreamIoT.Models
{
    public class SensorReading
    {
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; } = string.Empty;

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double SoilMoisture { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
