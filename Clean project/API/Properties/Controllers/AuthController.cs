using Application.Interfaces;
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
        // DTOs (Data Transfer Objects)
        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
