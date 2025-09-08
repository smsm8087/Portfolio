using DefenseGameWebServer.Data;
using DefenseGameWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace DefenseGameApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromForm] string username, [FromForm] string password, [FromForm] string nickname)
        {
            if (_db.Users.Any(u => u.Username == username))
                return Conflict(new { message = "already exist username" });

            string hashed = HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = hashed,
                Nickname = nickname
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new { message = "success", userId = user.Username, nickname = user.Nickname });
        }

        //암호화
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); 
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return Unauthorized(new { message = "not exist username" });

            string hashed = HashPassword(password);
            if (user.PasswordHash != hashed)
                return Unauthorized(new { message = "incorrect password" });

            return Ok(new
            {
                message = "success",
                userId = user.Username,
                nickname = user.Nickname
            });
        }
    }
}
