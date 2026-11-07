
using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SensorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/sensor
        [HttpGet]
        public async Task<IActionResult> GetReadings()
        {
            var readings = await _context.SensorReadings
                .OrderByDescending(r => r.RecordedAt)
                .ToListAsync();

            return Ok(readings);
        }

        // GET: api/sensor/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReading(int id)
        {
            var reading = await _context.SensorReadings.FindAsync(id);

            if (reading == null)
                return NotFound("Sensor reading not found.");

            return Ok(reading);
        }

        // POST: api/sensor
        [HttpPost]
        public async Task<IActionResult> AddReading(
            [FromBody] SensorReading reading)
        {
            reading.RecordedAt = DateTime.UtcNow;

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetReading),
                new { id = reading.Id },
                reading);
        }
    }
}
