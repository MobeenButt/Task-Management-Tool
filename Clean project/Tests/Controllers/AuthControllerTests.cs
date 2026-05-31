using API.Properties.Controllers;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new AuthController(_mockUserService.Object);
        }

        [Fact]
        public void Register_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var request = new AuthController.RegisterRequest
            {
                Username = "newuser",
                Password = "password123"
            };
            string expectedToken = "fake-jwt-token";
            _mockUserService.Setup(s => s.Register(request.Username, request.Password))
                           .Returns(expectedToken);

            // Act
            var result = _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            
            // Use reflection to check anonymous object properties
            var messageProperty = response.GetType().GetProperty("message");
            var tokenProperty = response.GetType().GetProperty("Token");
            var usernameProperty = response.GetType().GetProperty("username");
            var roleProperty = response.GetType().GetProperty("role");

            Assert.Equal("User registered successfully", messageProperty?.GetValue(response));
            Assert.Equal(expectedToken, tokenProperty?.GetValue(response));
            Assert.Equal(request.Username, usernameProperty?.GetValue(response));
            Assert.Equal("User", roleProperty?.GetValue(response));
        }

        [Fact]
        public void Register_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AuthController.RegisterRequest
            {
                Username = "existinguser",
                Password = "password123"
            };
            _mockUserService.Setup(s => s.Register(request.Username, request.Password))
                           .Throws(new Exception("Username already exists"));

            // Act
            var result = _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists", badRequestResult.Value);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var request = new AuthController.LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };
            
            // Create a real JWT token for testing
            string mockToken = CreateRealJwtToken("testuser", "User", "1");
            _mockUserService.Setup(s => s.Login(request.Username, request.Password))
                           .Returns(mockToken);

            // Act
            var result = _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            
            var messageProperty = response.GetType().GetProperty("message");
            var tokenProperty = response.GetType().GetProperty("Token");

            Assert.Equal("User logged in successfully", messageProperty?.GetValue(response));
            Assert.Equal(mockToken, tokenProperty?.GetValue(response));
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new AuthController.LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };
            _mockUserService.Setup(s => s.Login(request.Username, request.Password))
                           .Throws(new Exception("Invalid password"));

            // Act
            var result = _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = unauthorizedResult.Value;
            
            var messageProperty = response.GetType().GetProperty("message");
            Assert.Equal("Invalid password", messageProperty?.GetValue(response));
        }

        [Fact]
        public void GetProfile_WithValidUser_ShouldReturnUserInfo()
        {
            // Arrange
            SetupUserContext("1", "testuser", "User");

            // Act
            var result = _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            
            var userIdProperty = response.GetType().GetProperty("userId");
            var usernameProperty = response.GetType().GetProperty("username");
            var roleProperty = response.GetType().GetProperty("role");

            Assert.Equal(1, userIdProperty?.GetValue(response));
            Assert.Equal("testuser", usernameProperty?.GetValue(response));
            Assert.Equal("User", roleProperty?.GetValue(response));
        }

        [Fact]
        public void GetAllUsers_AsAdmin_ShouldReturnAllUsers()
        {
            // Arrange
            SetupUserContext("1", "admin", "Admin");
            var expectedUsers = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "admin", Role = "Admin" },
                new UserDto { Id = 2, Username = "user1", Role = "User" }
            };
            _mockUserService.Setup(s => s.GetAllUsers()).Returns(expectedUsers);

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsAssignableFrom<List<UserDto>>(okResult.Value);
            Assert.Equal(2, users.Count);
        }

        private void SetupUserContext(string userId, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        private string CreateRealJwtToken(string username, string role, string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("N9ccX0A6GC8GuHNZF7BAkozUI1r5raqwTestKey123456789"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TaskManagementAPI",
                audience: "TaskManagementClient",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}