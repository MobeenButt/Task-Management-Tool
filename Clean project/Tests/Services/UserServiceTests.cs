using Application.Interfaces;
using Infrastructure.UserServices;
using Microsoft.Extensions.Configuration;
using Tests.Helpers;
using Xunit;
using Moq;

namespace Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly Infrastructure.ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly Mock<IConfiguration> _mockConfig;

        public UserServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _mockConfig = new Mock<IConfiguration>();
            
            // Setup JWT configuration
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("N9ccX0A6GC8GuHNZF7BAkozUI1r5raqwTestKey123456789");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TaskManagementAPI");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TaskManagementClient");
            
            _userService = new UserService(_context, _mockConfig.Object);
        }

        [Fact]
        public void Register_WithValidData_ShouldCreateUserAndReturnToken()
        {
            // Arrange
            string username = "newuser";
            string password = "password123";

            // Act
            var token = _userService.Register(username, password);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            
            // Verify user was created
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            Assert.NotNull(user);
            Assert.Equal("User", user.Role);
            Assert.True(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash));
        }

        [Fact]
        public void Register_WithExistingUsername_ShouldThrowException()
        {
            // Arrange
            string existingUsername = "testuser"; // Already exists in test data
            string password = "password123";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _userService.Register(existingUsername, password));
            Assert.Equal("Username already exists", exception.Message);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            string username = "testuser";
            string password = "password123";

            // Act
            var token = _userService.Login(username, password);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void Login_WithInvalidUsername_ShouldThrowException()
        {
            // Arrange
            string invalidUsername = "nonexistentuser";
            string password = "password123";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _userService.Login(invalidUsername, password));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public void Login_WithInvalidPassword_ShouldThrowException()
        {
            // Arrange
            string username = "testuser";
            string invalidPassword = "wrongpassword";

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _userService.Login(username, invalidPassword));
            Assert.Equal("Invalid password", exception.Message);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            // Act
            var users = _userService.GetAllUsers();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Username == "testadmin");
            Assert.Contains(users, u => u.Username == "testuser");
        }

        [Fact]
        public void GetAllUsers_ShouldReturnUsersWithCorrectRoles()
        {
            // Act
            var users = _userService.GetAllUsers();

            // Assert
            var admin = users.FirstOrDefault(u => u.Username == "testadmin");
            var user = users.FirstOrDefault(u => u.Username == "testuser");
            
            Assert.NotNull(admin);
            Assert.Equal("Admin", admin.Role);
            
            Assert.NotNull(user);
            Assert.Equal("User", user.Role);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}