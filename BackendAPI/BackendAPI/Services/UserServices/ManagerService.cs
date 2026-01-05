using BackendAPI.Data.Entities.Users;
using BackendAPI.Repositories;


namespace BackendAPI.Services.UserServices
{
   

    public class ManagerService : BaseUserService<Manager>
    {
        public ManagerService(IUserRepository<Manager> repo) : base(repo) { }
    }

}
