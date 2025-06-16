namespace Role_Based_Invitation.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public int InviterId { get; set; }
        public User Inviter { get; set; }

        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }

        public string ReceiverEmail { get; set; }
        public string? GeneratedPassword { get; set; }
        public string Status { get; set; } // Pending, Accepted, Declined
    }

}
