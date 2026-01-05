
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

            // Validate vehicle ONLY if VehicleId is provided
            if (!string.IsNullOrEmpty(dto.VehicleId))
            {
                var vehicle = await _db.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == dto.VehicleId);

                if (vehicle == null)
                    throw new Exception("Vehicle not found");

                var alreadyAssigned = await _db.Drivers
                    .AnyAsync(d => d.VehicleId == dto.VehicleId);

                if (alreadyAssigned)
                    throw new Exception("Vehicle already assigned to another driver");
            }

            var driver = new Driver
            {
                Id = Nanoid.Generate(size: 10),       
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password, // TODO: hash later
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth.HasValue
                    ? DateTime.SpecifyKind(dto.DateOfBirth.Value, DateTimeKind.Utc)
                    : null,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,

                // may be null
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

       
        public async Task<DriverResponseDto> GetDriverByIdAsync(string id)
        {
            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                throw new Exception("Driver not found");

            return Map(driver);
        }

      
        public async Task UpdateStatusAsync(string id, string status)
        {
            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                throw new Exception("Driver not found");

            driver.Status = status;
            driver.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }


        public async Task AssignVehicleAsync(string driverId, string vehicleId)
        {
            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new Exception("Driver not found");

            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
                throw new Exception("Vehicle not found");

            var alreadyAssigned = await _db.Drivers
                .AnyAsync(d => d.VehicleId == vehicleId && d.Id != driverId);

            if (alreadyAssigned)
                throw new Exception("Vehicle already assigned to another driver");

            driver.VehicleId = vehicleId;
            driver.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }



        private static DriverResponseDto Map(Driver d)
        {
            return new DriverResponseDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Email = d.Email,
                Status = d.Status,
                CreatedAt = d.CreatedAt
            };
        }
    }
}
