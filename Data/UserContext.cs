using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quiz_API.Models;

namespace Quiz_API.Data
{
    public class UserContext : IdentityDbContext<User>
    {
        public UserContext(DbContextOptions options) : base(options)
        {
        }
    }
}
