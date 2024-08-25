using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using inventory_system.Controllers;
using inventory_system.Models;
using inventory_system.Services.Auth;

namespace inventory_system.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            var user = new User { Username = "testuser", Password = "password" };
            _authServiceMock.Setup(s => s.RegisterUser(user)).ReturnsAsync(true);

            var result = await _controller.Register(user) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Register_UsernameExists_ReturnsConflict()
        {
            var user = new User { Username = "testuser", Password = "password" };
            _authServiceMock.Setup(s => s.RegisterUser(user)).ReturnsAsync(false);

            var result = await _controller.Register(user) as ConflictObjectResult;

            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var model = new LoginModel { Username = "testuser", Password = "password" };
            var accessToken = "accessToken";
            var refreshToken = "refreshToken";
            _authServiceMock.Setup(s => s.LoginUser(model.Username, model.Password))
                .ReturnsAsync((accessToken, refreshToken));

            var result = await _controller.Login(model) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var model = new LoginModel { Username = "testuser", Password = "wrongpassword" };
            _authServiceMock.Setup(s => s.LoginUser(model.Username, model.Password))
                .ReturnsAsync((null, null));

            var result = await _controller.Login(model) as UnauthorizedObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_ValidToken_ReturnsOk()
        {
            var model = new RefreshTokenModel { RefreshToken = "validToken" };
            var accessToken = "newAccessToken";
            var refreshToken = "newRefreshToken";
            _authServiceMock.Setup(s => s.RefreshToken(model.RefreshToken))
                .ReturnsAsync((accessToken, refreshToken));

            var result = await _controller.RefreshToken(model) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ReturnsBadRequest()
        {
            var model = new RefreshTokenModel { RefreshToken = "invalidToken" };
            _authServiceMock.Setup(s => s.RefreshToken(model.RefreshToken))
                .ReturnsAsync((null, null));

            var result = await _controller.RefreshToken(model) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

    }
}