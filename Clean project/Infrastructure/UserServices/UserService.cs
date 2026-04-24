using Application.Interfaces;
using Domain.Entites;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.UserServices
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public UserService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public string Register(string username, string password)
        {
            var checkUser = _context.Users.FirstOrDefault(u => u.Username == username);
            if (checkUser != null)
            {
                throw new Exception("User already exists");
            }
            //Hash password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Role = "User"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Generate JWT token
            return GenerateToken(user);
        }

        public string Login(string username, string password)
        {
            //find user by username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            //Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid password");
            }

            return GenerateToken(user);
        }
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
