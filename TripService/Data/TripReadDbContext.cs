using Microsoft.EntityFrameworkCore;
using TripService.Models;

namespace TripService.Data;

public class TripReadDbContext : DbContext
{
    public TripReadDbContext(DbContextOptions<TripReadDbContext> opts) : base(opts) { }
    public DbSet<Trip> Trips => Set<Trip>();
}

