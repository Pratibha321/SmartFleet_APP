using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripService.Data;
using TripService.Models;

namespace TripService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripCommandsController : ControllerBase
{
    private readonly TripWriteDbContext _write;
    private readonly TripReadDbContext _read;
    private readonly IdempotencyDbContext _idemp;
    private readonly ILogger<TripCommandsController> _logger;

    public TripCommandsController(TripWriteDbContext write, TripReadDbContext read, IdempotencyDbContext idemp, ILogger<TripCommandsController> logger)
    {
        _write = write; _read = read; _idemp = idemp; _logger = logger;
    }

    [HttpPost]
    //[Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> Create([FromBody] Trip dto)
    {
        // Accept TripReference as business idempotency token, plus optional Idempotency-Key header
        var idempKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(idempKey))
        {
            var rec = await _idemp.IdempotencyKeys.FindAsync(new object[] { idempKey });
            if (rec != null) return StatusCode(rec.ResponseStatus, rec.ResponseBody);
        }

        // Business-level idempotency: TripReference uniqueness in write DB
        var existing = await _write.Trips.FirstOrDefaultAsync(t => t.TripReference == dto.TripReference);
        if (existing != null)
        {
            _logger.LogInformation("Trip create called with existing TripReference {r}", dto.TripReference);
            return Conflict(new { message = "Trip already exists", id = existing.Id });
        }

        dto.Id = Guid.NewGuid();
        dto.CreatedAt = DateTime.UtcNow;
        _write.Trips.Add(dto);
        await _write.SaveChangesAsync();

        // Project to read DB
        _read.Trips.Add(dto);
        await _read.SaveChangesAsync();

        var resp = new { id = dto.Id, tripReference = dto.TripReference };

        if (!string.IsNullOrWhiteSpace(idempKey))
        {
            _idemp.IdempotencyKeys.Add(new IdempotencyRecord
            {
                Key = idempKey,
                ResponseStatus = 201,
                ResponseBody = System.Text.Json.JsonSerializer.Serialize(resp),
                CreatedAt = DateTime.UtcNow
            });
            await _idemp.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetStatus), "TripQueries", new { id = dto.Id }, resp);
    }

    [HttpPut("{id:guid}/status")]
   // [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var idempKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(idempKey))
        {
            var rec = await _idemp.IdempotencyKeys.FindAsync(new object[] { idempKey });
            if (rec != null) return StatusCode(rec.ResponseStatus, rec.ResponseBody);
        }

        if (!Enum.TryParse<TripStatus>(status, true, out var s))
            return BadRequest("Invalid status");

        var t = await _write.Trips.FindAsync(id);
        if (t == null) return NotFound();

        t.Status = s;
        t.UpdatedAt = DateTime.UtcNow;
        await _write.SaveChangesAsync();

        // Update read projection
        var r = await _read.Trips.FindAsync(id);
        if (r != null) { r.Status = t.Status; r.UpdatedAt = t.UpdatedAt; await _read.SaveChangesAsync(); }

        var resp = t;

        if (!string.IsNullOrWhiteSpace(idempKey))
        {
            _idemp.IdempotencyKeys.Add(new IdempotencyRecord
            { Key = idempKey, ResponseStatus = 200, ResponseBody = System.Text.Json.JsonSerializer.Serialize(resp), CreatedAt = DateTime.UtcNow });
            await _idemp.SaveChangesAsync();
        }

        return Ok(resp);
    }

    // quick status helper (used as CreatedAtAction target)
    [HttpGet("status/{id:guid}")]
    [AllowAnonymous]
    public IActionResult GetStatus(Guid id) => Ok(new { id });
}