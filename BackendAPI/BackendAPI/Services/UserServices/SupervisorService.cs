using BackendAPI.Data.Entities.Users;
using BackendAPI.Repositories;

namespace BackendAPI.Services.UserServices
{
    
    public class SupervisorService : BaseUserService<Supervisor>
    {
        public SupervisorService(IUserRepository<Supervisor> repo) : base(repo) { }
    }

}
