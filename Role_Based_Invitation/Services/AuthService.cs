using Role_Based_Invitation.Data;
using Role_Based_Invitation.Models;
using System.Security.Cryptography;
using System.Text;

namespace Role_Based_Invitation.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        public AuthService(AppDbContext db) => _db = db;

        public string HashPassword(string password) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        public User Authenticate(string email, string password)
        {
            var hash = HashPassword(password);
            return _db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
        }
    }

}
