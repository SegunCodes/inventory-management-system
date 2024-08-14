using inventory_system.Models;
using inventory_system.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventory_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var result = await _authService.RegisterUser(user);
            if (!result)
            {
                return Conflict("Username already exists.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var (accessToken, refreshToken) = await _authService.LoginUser(model.Username, model.Password);
            if (accessToken == null)
            {
                return Unauthorized();
            }

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var (accessToken, refreshToken) = await _authService.RefreshToken(model.RefreshToken);
            if (accessToken == null)
            {
                return BadRequest("Invalid refresh token");
            }

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }
    }
}