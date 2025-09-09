using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quiz_API.Data;
using Quiz_API.Enums;
using Quiz_API.Models;
using Quiz_API.Repository;
using Quiz_API.Dto;
using Quiz_API.Services;
namespace Quiz_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserRepository _userRepository;
        public readonly JwtService _jwtService;
        public UserController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            var result = await _userRepository.CreateUser(request.email, request.username, request.password);
            return result switch
            {
                UserRepositoryResult.Succes => Ok("User Created"),
                UserRepositoryResult.UserNotCreated => BadRequest("User not created"),
                _ => StatusCode(500, "Unexpected error.")
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try {
                var checkUser = await _userRepository.Login(request.email, request.password);
                if (checkUser == UserRepositoryResult.UserNotFound)
                {
                    return BadRequest("User not found.");
                }
                if (checkUser == UserRepositoryResult.WrongPassword)
                {
                    return BadRequest("Wrong password.");
                }
                var user = await _userRepository.GetUserFromCredentials(request.email, request.password);
                var token = _jwtService.GenerateToken(user);
                return Ok(new { token });

            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, title: "Błąd serwera");
            }
        }
    }
}
