using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTO.Driver;

using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace BackendAPI.Services
{
    public class DriverService
    {
        private readonly AppDbContext _db;

        public DriverService(AppDbContext db)
        {
            _db = db;
        }

        
        public async Task<DriverResponseDto> CreateDriverAsync(CreateDriverDto dto)
        {
            if (await _db.Drivers.AnyAsync(d => d.Email == dto.Email))
                throw new Exception("Driver with this email already exists");

            var vehicle = await _db.Vehicles
        .FirstOrDefaultAsync(v => v.Id == dto.VehicleId);

            if (vehicle == null)
                throw new Exception("Vehicle not found");

            var driver = new Driver
            {
                Id = Nanoid.Generate(size: 10),       
                //DriverId = Nanoid.Generate(size: 10),
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password, // hash later
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth.HasValue
    ? DateTime.SpecifyKind(dto.DateOfBirth.Value, DateTimeKind.Utc)
    : null,
                Status = DriverStatus.Active,
                CreatedAt = DateTime.UtcNow,
                VehicleId = dto.VehicleId
            };

            _db.Drivers.Add(driver);
            await _db.SaveChangesAsync();

            return Map(driver);
        }

       
        public async Task<List<DriverResponseDto>> GetDriversAsync()
        {
            return await _db.Drivers
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => Map(d))
                .ToListAsync();
        }

        public async Task<DriverResponseDto> GetDriverByIdAsync(string driverId)
        {
            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new Exception("Driver not found");

            return Map(driver);
        }

    
        public async Task UpdateStatusAsync(string driverId, DriverStatus status)
        {
            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new Exception("Driver not found");

            driver.Status = status;
            driver.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        private static DriverResponseDto Map(Driver d)
        {
            return new DriverResponseDto
            {
                DriverId = d.Id,
                FullName = d.FullName,
                Email = d.Email,
                Status = d.Status,
                CreatedAt = d.CreatedAt
            };
        }
    }
}
