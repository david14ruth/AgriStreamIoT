using AgriStreamIoT.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<IrrigationCommand> IrrigationCommands { get; set; }
    }
}