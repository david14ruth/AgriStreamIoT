
using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IrrigationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IrrigationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Irrigation
        [HttpGet]
        public async Task<IActionResult> GetCommands()
        {
            var commands = await _context.IrrigationCommands
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(commands);
        }

        // GET: api/Irrigation/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommand(int id)
        {
            var command = await _context.IrrigationCommands
                .FindAsync(id);

            if (command == null)
                return NotFound("Irrigation command not found.");

            return Ok(command);
        }

        // POST: api/Irrigation
        [HttpPost]
        public async Task<IActionResult> AddCommand(
            [FromBody] IrrigationCommand command)
        {
            command.Id = 0;
            command.CreatedAt = DateTime.UtcNow;
            command.Status = "Pending";

            _context.IrrigationCommands.Add(command);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCommand),
                new { id = command.Id },
                command);
        }

        // POST: api/Irrigation/1/process
        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessCommand(int id)
        {
            var command = await _context.IrrigationCommands
                .FindAsync(id);

            if (command == null)
            {
                return NotFound("Irrigation command not found.");
            }

            if (command.Status != "Pending")
            {
                return Conflict("This irrigation command has already been processed.");
            }

            if (command.Command != "ON" && command.Command != "OFF")
            {
                command.Status = "Failed";

                await _context.SaveChangesAsync();

                return BadRequest("Invalid irrigation command. Use ON or OFF.");
            }

            // Simulate successful command processing
            command.Status = "Completed";

            await _context.SaveChangesAsync();

            return Ok(command);
        }
    }
}
