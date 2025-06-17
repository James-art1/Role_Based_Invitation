using Microsoft.AspNetCore.Mvc;
using Role_Based_Invitation.Data;
using Role_Based_Invitation.Models;
using Role_Based_Invitation.Services;

namespace Role_Based_Invitation.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public AccountController(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string username, string email, string password, string organizationName)
        {
            if (_db.Users.Any(u => u.Email == email)) return BadRequest("Email already exists");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = _auth.HashPassword(password),
                Role = Role.Admin,
                OrganizationName = organizationName
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());

            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _auth.Authenticate(email, password);
            if (user == null) return Unauthorized();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            return user.Role == Role.Admin
                ? RedirectToAction("ManageUsers", "Invite")
                : RedirectToAction("Welcome", "Invite");

        }
    }

}
