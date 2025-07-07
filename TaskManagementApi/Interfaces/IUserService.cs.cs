using TaskManagementApi.Models;

namespace TaskManagementApi.Interfaces
{
    public interface IUserService
    {
        System.Threading.Tasks.Task<User> GetUserByUsernameAsync(string username);
        System.Threading.Tasks.Task<bool> VerifyPasswordAsync(User user, string password);
        System.Threading.Tasks.Task AddUserAsync(User user, string password);
    }
}
