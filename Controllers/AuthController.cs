using inventory_system.Models;
using inventory_system.Services;
using inventory_system.Services.Auth;
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
                return BadRequest(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Username and password are required." 
                });
            }

            var result = await _authService.RegisterUser(user);
            if (!result)
            {
                 return Conflict(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Username already exists." 
                });
            }

            return Ok(new ApiResponse 
            { 
                Success = true, 
                Message = "User registered successfully.",
                Data = new { user.Username }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var (accessToken, refreshToken) = await _authService.LoginUser(model.Username, model.Password);
            if (accessToken == null)
            {
                return Unauthorized(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Invalid username or password." 
                });
            }

            return Ok(new ApiResponse 
            { 
                Success = true, 
                Message = "Login successful.",
                Data = new { accessToken, refreshToken }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var (accessToken, refreshToken) = await _authService.RefreshToken(model.RefreshToken);
            if (accessToken == null)
            {
                return BadRequest(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Invalid refresh token." 
                });
            }

            return Ok(new ApiResponse 
            { 
                Success = true, 
                Message = "Token refreshed successfully.",
                Data = new { accessToken, refreshToken }
            });
        }
    }
}