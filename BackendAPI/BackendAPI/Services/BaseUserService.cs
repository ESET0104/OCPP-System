using BackendAPI.Data.Entities.Users;
using BackendAPI.DTO.Auth;
using BackendAPI.Repositories;
using BackendAPI.Exceptions;

namespace BackendAPI.Services
{
    public class BaseUserService<TUser>
        where TUser : class, IUser, new()
    {
        protected readonly IUserRepository<TUser> _repo;

        public BaseUserService(IUserRepository<TUser> repo)
        {
            _repo = repo;
        }

        

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            return (await _repo.GetAllAsync()).Select(Map);
        }

        

        public async Task<UserDTO> GetByIdAsync(string id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
                throw new BusinessException("User not found");

            return Map(user);
        }

        

        public async Task<UserDTO> CreateAsync(CreateUserDto dto)
        {
            

           
            if (await _repo.ExistsAsync(u => u.Username == dto.Username))
                throw new BusinessException("Username already exists");

            
            if (await _repo.ExistsAsync(u => u.Email == dto.Email))
                throw new BusinessException("Email already exists");

            var user = new TUser
            {
                Id = Guid.NewGuid().ToString(),
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = dto.Status,
                Company = dto.Company,
                Department = dto.Department,
                CreatedAt = DateTime.UtcNow,
                Tokenat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            await _repo.CreateAsync(user);
            return Map(user);
        }

        

        public async Task<UserDTO> UpdateAsync(string id, UpdateUserDto dto)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
                throw new BusinessException("User not found");

            
            if (!string.IsNullOrEmpty(dto.Username) &&
                await _repo.ExistsAsync(u => u.Username == dto.Username && u.Id != id))
                throw new BusinessException("Username already exists");

            
            if (!string.IsNullOrEmpty(dto.Email) &&
                await _repo.ExistsAsync(u => u.Email == dto.Email && u.Id != id))
                throw new BusinessException("Email already exists");

            user.Username = dto.Username ?? user.Username;
            user.Email = dto.Email ?? user.Email;
            user.Status = dto.Status ?? user.Status;
            user.Company = dto.Company ?? user.Company;
            user.Department = dto.Department ?? user.Department;
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user);
            return Map(user);
        }

        

        public async Task<UserDTO> PatchAsync(string id, UpdateUserDto dto)
        {
            return await UpdateAsync(id, dto);
        }


        public async Task DeleteAsync(string id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
                throw new BusinessException("User not found");

            await _repo.DeleteAsync(user);
        }

        

        protected virtual UserDTO Map(TUser u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Status = u.Status,
            Company = u.Company,
            Department = u.Department,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            LastActiveAt = u.LastActiveAt
        };
    }
}
