using BackendAPI.Data.Entities.Users;
using BackendAPI.Repositories;

namespace BackendAPI.Services.UserServices
{
   
    public class AdminService : BaseUserService<Admin>
    {
        public AdminService(IUserRepository<Admin> repo) : base(repo) { }
    }

}
