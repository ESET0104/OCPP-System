using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    private readonly AppDbContext _context;

    public LocationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationRequest request)

    {
        var location = new Location
        {
            Id = Nanoid.Generate(size: 10),   
            Name = request.Name,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return Ok(location);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Locations.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound("Location not found");

        return Ok(location);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Location updatedLocation)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound("Location not found");

        location.Name = updatedLocation.Name;
        location.Address = updatedLocation.Address;
        location.Latitude = updatedLocation.Latitude;
        location.Longitude = updatedLocation.Longitude;

        await _context.SaveChangesAsync();
        return Ok(location);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound("Location not found");

        bool hasChargers = await _context.Chargers
            .AnyAsync(c => c.LocationId == id);

        if (hasChargers)
            return BadRequest("Cannot delete location. Chargers are assigned to it.");

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return Ok("Location deleted successfully");
    }

    [HttpGet("with-chargers")]
    public async Task<IActionResult> GetAllWithChargers()
    {
        var result = await _context.Locations
            .Select(l => new
            {
                l.Id,
                l.Name,
                l.Address,
                l.Latitude,
                l.Longitude,
                Chargers = _context.Chargers
                    .Where(c => c.LocationId == l.Id)
                    .Select(c => new
                    {
                        c.Id,
                        c.Status,
                        c.LastSeen
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search term is required");

        var locations = await _context.Locations
            .Where(l => l.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();

        return Ok(locations);
    }

    [HttpGet("{id}/chargers")]
    public async Task<IActionResult> GetLocationWithChargers(string id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound("Location not found");

        var chargers = await _context.Chargers
            .Where(c => c.LocationId == id)
            .Select(c => new
            {
                c.Id,
                c.Status,
                c.LastSeen
            })
            .ToListAsync();

        return Ok(new
        {
            location.Id,
            location.Name,
            location.Address,
            location.Latitude,
            location.Longitude,
            Chargers = chargers
        });
    }
}
