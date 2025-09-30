using Microsoft.EntityFrameworkCore;
using NotificationService.Models;
using System.Collections.Generic;

namespace NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> opts) : base(opts) { }
    public DbSet<Notification> Notifications => Set<Notification>();
}