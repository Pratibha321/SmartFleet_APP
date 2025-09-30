
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelemetryService.Data;

namespace TelemetryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryDbContext _db;
    public TelemetryController(TelemetryDbContext db) => _db = db;

    [HttpGet("latest/{vehicleId}")]
    public async Task<IActionResult> Latest(string vehicleId)
    {
        var latest = await _db.Telemetry.Where(t => t.VehicleId == vehicleId)
                                        .OrderByDescending(t => t.Timestamp)
                                        .Take(50)
                                        .ToListAsync();
        return Ok(latest);
    }
}