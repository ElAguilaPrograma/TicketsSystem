using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsSystem.Core.Services;
using TicketsSystem.Data.DTOs;

namespace TicketsSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpPost("createuser")]
        public async Task<IActionResult> CreateNewUser([FromBody] UserDTO userDTO)
        {
            await _userService.CreateNewUserAsync(userDTO);

            return Ok(new
            {
                Success = true,
                Message = "User created"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string result = await _userService.LoginAsync(request);
            return Ok(result);
        }
    }
}
