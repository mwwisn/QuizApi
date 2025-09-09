using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quiz_API.Data;
using Quiz_API.Dto;
using Quiz_API.Enums;
using Quiz_API.Models;

namespace Quiz_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserRepositoryResult> CreateUser(string email, string username, string password)
        {
            var user = new User
            {
                UserName = username,
                Email = email
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return UserRepositoryResult.Succes;
            }
                return UserRepositoryResult.UserNotCreated;
        }
         public async Task<UserRepositoryResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return UserRepositoryResult.UserNotFound;
            }
            var pass = await _userManager.CheckPasswordAsync(user, password);
            if(!pass)
            {
                return UserRepositoryResult.WrongPassword;
            }
            return UserRepositoryResult.Succes;

        }
        public async Task<User> GetUserFromCredentials(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }
    }
}
