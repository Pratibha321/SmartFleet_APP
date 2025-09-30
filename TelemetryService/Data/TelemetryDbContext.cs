using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TelemetryService.Models;

namespace TelemetryService.Data;

public class TelemetryDbContext : DbContext
{
    public TelemetryDbContext(DbContextOptions<TelemetryDbContext> opts) : base(opts) { }
    public DbSet<TelemetryRecord> Telemetry => Set<TelemetryRecord>();
}