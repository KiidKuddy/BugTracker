using System.Threading.Tasks;
using BugTracker_API.Models;

namespace BugTracker_API.Data
{
    public interface IAuthenticationRepository
    {
         Task<User> RegisterUser(User user, string password);
         Task<User> LoginUser(string email, string password);
         Task<bool> EmailExists(string email);
    }
}