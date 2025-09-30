using Microsoft.EntityFrameworkCore;

namespace TripService.Data;

public class IdempotencyRecord
{
    public string Key { get; set; } = string.Empty;
    public int ResponseStatus { get; set; }
    public string ResponseBody { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class IdempotencyDbContext : DbContext
{
    public IdempotencyDbContext(DbContextOptions<IdempotencyDbContext> opts) : base(opts) { }
    public DbSet<IdempotencyRecord> IdempotencyKeys => Set<IdempotencyRecord>();
}