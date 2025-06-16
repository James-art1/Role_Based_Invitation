using Microsoft.EntityFrameworkCore;
using Role_Based_Invitation.Models;

namespace Role_Based_Invitation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<PractExam> PracticalExaminations { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Inviter)
                .WithMany(u => u.SentInvites)
                .HasForeignKey(i => i.InviterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Receiver)
                .WithMany(u => u.ReceivedInvites)
                .HasForeignKey(i => i.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
