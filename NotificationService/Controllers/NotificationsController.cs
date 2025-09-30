using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _db;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(NotificationDbContext db, ILogger<NotificationsController> logger)
    {
        _db = db; _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] Notification n)
    {
        n.SentAt = DateTime.UtcNow;
        _db.Notifications.Add(n);
        await _db.SaveChangesAsync();

        // simulate external provider
        if (n.Type == NotificationType.Email)
            _logger.LogInformation("[Email] to {r} : {m}", n.Recipient, n.Message);
        else
            _logger.LogInformation("[SMS] to {r} : {m}", n.Recipient, n.Message);

        return Ok(new { ok = true, id = n.Id });
    }

    [HttpGet]
    public async Task<IActionResult> All() => Ok(await _db.Notifications.ToListAsync());
}
