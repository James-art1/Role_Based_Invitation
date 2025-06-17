using System.ComponentModel;

namespace Role_Based_Invitation.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public string OrganizationName { get; set; }

        public ICollection<Invite> SentInvites { get; set; }
        public ICollection<Invite> ReceivedInvites { get; set; }
    }

    public enum Role
    {
        Admin = 0,
        User = 1
    }
}
