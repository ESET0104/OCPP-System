using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTO.Vehicle;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace BackendAPI.Services
{
    public class VehicleService
    {
        private readonly AppDbContext _db;

        public VehicleService(AppDbContext db)
        {
            _db = db;
        }

      
        public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto)
        {
            if (await _db.Vehicles.AnyAsync(v => v.VIN == dto.VIN))
                throw new Exception("Vehicle with this VIN already exists");

            var vehicle = new Vehicle
            {
                Id = Nanoid.Generate(size: 10),
                VehicleName = dto.VehicleName,
                VIN = dto.VIN,
                MakeandModel = $"{dto.Make} {dto.Model} {dto.Variant}",
                RegistrationNumber = dto.RegistrationNumber,
                RangeKm = dto.RangeKm
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            return await MapAsync(vehicle);
        }

    
        public async Task<List<VehicleResponseDto>> GetAllAsync()
        {
            var vehicles = await _db.Vehicles.ToListAsync();
            var result = new List<VehicleResponseDto>();

            foreach (var v in vehicles)
            {
                result.Add(await MapAsync(v));
            }

            return result;
        }


        public async Task<VehicleResponseDto> GetByIdAsync(string vehicleId)
        {
            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
                throw new Exception("Vehicle not found");

            return await MapAsync(vehicle);
        }


        public async Task<VehicleResponseDto> UpdateAsync(
    string vehicleId,
    UpdateVehicleDto dto)
        {
            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
                throw new Exception("Vehicle not found");

            vehicle.VehicleName = dto.VehicleName;
            vehicle.MakeandModel = $"{dto.Make} {dto.Model} {dto.Variant}";
            vehicle.RegistrationNumber = dto.RegistrationNumber;
            vehicle.RangeKm = dto.RangeKm;

            await _db.SaveChangesAsync();

            return await MapAsync(vehicle);
        }


        public async Task DeleteAsync(string vehicleId)
        {
            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
                throw new Exception("Vehicle not found");

            // Prevent deleting vehicle if linked to a driver
            var isAssigned = await _db.Drivers.AnyAsync(d => d.VehicleId == vehicleId);
            if (isAssigned)
                throw new Exception("Vehicle is assigned to a driver");

            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
        }


        private async Task<VehicleResponseDto> MapAsync(Vehicle v)
        {
            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.VehicleId == v.Id);

            return new VehicleResponseDto
            {
                VehicleId = v.Id,
                DriverId = driver?.Id,   // nullable
                VehicleName = v.VehicleName,
                Type = "EV",
                Make = ExtractMake(v.MakeandModel),
                Model = ExtractModel(v.MakeandModel),
                Variant = ExtractVariant(v.MakeandModel),
                RegistrationNumber = v.RegistrationNumber,
                VIN = v.VIN,
                RangeKm = v.RangeKm
            };
        }

        private static string ExtractMake(string value) =>
            value.Split(' ').FirstOrDefault() ?? "";

        private static string ExtractModel(string value) =>
            value.Split(' ').Skip(1).FirstOrDefault() ?? "";

        private static string ExtractVariant(string value) =>
            value.Split(' ').Skip(2).FirstOrDefault() ?? "";
    }
}
