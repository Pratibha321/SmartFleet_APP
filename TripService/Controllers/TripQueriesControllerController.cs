using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripService.Data;

namespace TripService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripQueriesController : ControllerBase
{
    private readonly TripReadDbContext _read;
    public TripQueriesController(TripReadDbContext read) => _read = read;

    [HttpGet]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> GetAll() => Ok(await _read.Trips.ToListAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Dispatcher,Driver")]
    public async Task<IActionResult> Get(Guid id)
    {
        var t = await _read.Trips.FindAsync(id);
        return t == null ? NotFound() : Ok(t);
    }
}

//Endpoints/TelemetryMonitor.cs(lightweight background worker skeleton)
// optional: background worker that could consume telemetry, evaluate rules and call NotificationService
// Implementation omitted here — we keep the hook to add a hosted service if you want
