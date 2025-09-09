using System.ComponentModel.DataAnnotations;

namespace Quiz_API.Dto
{
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
