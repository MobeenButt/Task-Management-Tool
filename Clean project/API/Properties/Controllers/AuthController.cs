using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody]RegisterRequest request)
        {
            try
            {
                var token = _userService.Register(request.Username,request.Password);
                return Ok(new
                {
                    message = "User registered successfully",
                    Token = token,
                    username = request.Username,
                    role = "User"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = _userService.Login(request.Username, request.Password);

                // decoding token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var username = jwtToken.Claims.First(u => u.Type == ClaimTypes.Name).Value;
                var role = jwtToken.Claims.First(r => r.Type == ClaimTypes.Role).Value;


                return Ok(new
                {
                    message = "User logged in successfully",
                    Token = token,
                    username=username,
                    role=role
                    
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("profile")]
        public IActionResult GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found in token."));
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("Username not found in token.");
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? throw new Exception("Role not found in token.");

                return Ok(new
                {
                    userId = userId,
                    username = username,
                    role = role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DTOs (Data Transfer Objects)
        public class RegisterRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        public class LoginRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }
    }
}
