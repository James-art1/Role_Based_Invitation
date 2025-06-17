using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Role_Based_Invitation.Models
{
    //public class Invite
    //{
    //    public int Id { get; set; }
    //    public int InviterId { get; set; }
    //    public User Inviter { get; set; }

    //    public int? ReceiverId { get; set; }
    //    public User Receiver { get; set; }

    //    public string ReceiverEmail { get; set; }
    //    public string? GeneratedPassword { get; set; }
    //    public string Status { get; set; } 
    //}

    public class Invite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InviterId { get; set; }

        [ForeignKey(nameof(InviterId))]
        public User Inviter { get; set; }

        public int? ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public User Receiver { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string ReceiverEmail { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [MaxLength(256)]
        public string InvitationToken { get; set; }  // Assuming you store this in DB

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddDays(7);
    }
}
