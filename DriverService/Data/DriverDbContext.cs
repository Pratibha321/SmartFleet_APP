using DriverService.Models;
using Microsoft.EntityFrameworkCore;
namespace DriverService.Data
{
    public class DriverDbContext : DbContext
    {
        public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
        {
                
        }
        public DbSet<Driver> Drivers { get; set; }
    }
}
