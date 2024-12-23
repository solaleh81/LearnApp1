using LearnApp.Data;
using LearnApp.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;


namespace LearnApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Name, email, and password are required.");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }

            var passwordHash = HashPassword(model.Password);
            Console.WriteLine(passwordHash);

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful", userId = user.Id });
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var passwordHash = HashPassword(model.Password);
            if (user.PasswordHash != passwordHash)
            {
                return Unauthorized("Invalid email or password.");
            }

            try
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while generating token: {ex.Message}");
            }
        }



        private string GenerateJwtToken(User user)
        {

            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Name) || user.Id == 0)
            {
                throw new ArgumentException("User object is invalid or missing required properties.");
            }

            var secretKey = "MySuperSecretKey12345!@#$%SecureJWTKey";
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }

            var issuer = "http://localhost";
            var audience = "API Users";
            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("Issuer or Audience is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            };

            var expireInMinutesStr = "1";
            if (string.IsNullOrEmpty(expireInMinutesStr))
            {
                throw new InvalidOperationException("ExpireInMinutes is not configured or is empty.");
            }

            if (!int.TryParse(expireInMinutesStr, out int expireInMinutes))
            {
                throw new InvalidOperationException("ExpireInMinutes is not a valid integer.");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireInMinutes),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };


            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}

