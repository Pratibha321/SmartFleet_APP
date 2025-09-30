using DriverService.Data;
using DriverService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriverService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly DriverDbContext _driverDbContext;
        public DriverController(DriverDbContext driverDbContext)
        {
            _driverDbContext=driverDbContext;    
        }

        [HttpGet]
       // [Authorize(Roles = "Admin,Dispatcher")]
        public async Task<IActionResult> GetAll()
        {
            var driver = await _driverDbContext.Drivers.ToListAsync();
            return Ok(driver);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Dispatcher")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var d = await _driverDbContext.Drivers.FindAsync(id);
            return d==null ? NotFound() : Ok(d);
        }

        [HttpPost]
      //  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Driver dto)
        {
            dto.Id = Guid.NewGuid();

            await _driverDbContext.Drivers.AddAsync(dto); 
            await _driverDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        [HttpPut]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Driver dto)
        {
            var d = await _driverDbContext.Drivers.FindAsync(id);
            if (d == null) return NotFound();
            d.FullName = dto.FullName;
            d.Phone = dto.Phone;
            d.LicenseNumber = dto.LicenseNumber;
            d.LicenseExpiry = dto.LicenseExpiry;
            d.Role = dto.Role;
            await _driverDbContext.SaveChangesAsync();
            return Ok(d);
        }

        [HttpDelete("id:guid")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var driver = await _driverDbContext.Drivers.FindAsync(id);
            if (driver==null) NotFound();

             _driverDbContext.Drivers.Remove(driver);
            await _driverDbContext.SaveChangesAsync();

            return NoContent();

        }



    }
}
