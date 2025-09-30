using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> opts) : base(opts) { }
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
}