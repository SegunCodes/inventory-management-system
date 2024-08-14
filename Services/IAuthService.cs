using inventory_system.Models;

namespace inventory_system.Services
{
    public interface IAuthService
    {
       Task<bool> RegisterUser(User user);
        Task<(string accessToken, string refreshToken)> LoginUser(string username, string password);
        Task<(string accessToken, string refreshToken)> RefreshToken(string token);
        string GenerateJwtToken(User user);
    }
}