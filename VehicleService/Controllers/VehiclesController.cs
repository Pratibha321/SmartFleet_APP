
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using VehicleService.Data;
using VehicleService.Models;

namespace VehicleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly VehicleDbContext _dbContext;
        public VehiclesController(VehicleDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        [HttpGet]
       // [Authorize(Roles = "Admin,Dispatcher")]
        public async Task<IActionResult> GetAllVehicles()
        {
            var list = await _dbContext.Vehicles.ToListAsync();
            return Ok(list);
        }

        [HttpGet("id:guid")]
       // [Authorize(Roles = "Admin,Dispatcher")]
        public async Task<IActionResult> GetVehiclesByID(Guid id)
        {
            var v = await _dbContext.Vehicles.FindAsync(id);

            return v == null ? NotFound() : Ok(v);

        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Vehicle v )
        {
            v.Id= new Guid();
            v.IsActive=true;
            _dbContext.Add(v);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVehiclesByID), new { id = v.Id }, v);
        }

        [HttpPut]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Vehicle input)
        {
            var v = await _dbContext.Vehicles.FindAsync(id);
            if (v==null) return NotFound();
            v.RegistrationNumber=input.RegistrationNumber;
            v.Type=input.Type;
            v.Capacity=input.Capacity;
            v.LastMaintenance=input.LastMaintenance;
            v.IsActive=input.IsActive;
            await _dbContext.SaveChangesAsync();
            return Ok(v);
        }

        [HttpDelete("id:guid")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vehicles = await _dbContext.Vehicles.FindAsync(id);
            if (vehicles==null) NotFound();

            _dbContext.Vehicles.Remove(vehicles);
            await _dbContext.SaveChangesAsync();

            return NoContent();

        }


    }
}
