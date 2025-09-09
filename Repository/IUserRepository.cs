using Quiz_API.Enums;
using Quiz_API.Models;

namespace Quiz_API.Repository
{
    public interface IUserRepository
    {
        public Task<UserRepositoryResult> CreateUser(string email, string username, string password);
        public Task<UserRepositoryResult> Login(string email, string password);
        public Task<User> GetUserFromCredentials(string email, string password);
    }
}
