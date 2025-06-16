using System.ComponentModel;

namespace Role_Based_Invitation.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Role { get; set; } // "Admin" or "User"
        public string OrganizationName { get; set; }

        public ICollection<Invite> SentInvites { get; set; }
        public ICollection<Invite> ReceivedInvites { get; set; }
    }

}
