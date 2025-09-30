using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TripService.Models;

namespace TripService.Data;

public class TripWriteDbContext : DbContext
{
    public TripWriteDbContext(DbContextOptions<TripWriteDbContext> opts) : base(opts) { }
    public DbSet<Trip> Trips => Set<Trip>();
}
