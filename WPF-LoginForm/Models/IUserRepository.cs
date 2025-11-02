using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace WPF_LoginForm.Models
{
    public interface IUserRepository
    {
        Task<LoginResponse> AuthenticateUserAsync(string email, string password);
        Task AddAsync(UserModel userModel);
        Task EditAsync(UserModel userModel);
        Task RemoveAsync(int id);
        Task<UserModel> GetByIdAsync(int id);
        Task<UserModel> GetByUsernameAsync(string username);
        Task<IEnumerable<UserModel>> GetAllAsync();
    }
}
