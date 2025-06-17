namespace Role_Based_Invitation.Models
{
    public class PractExam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public int CreatedByAdminId { get; set; }

    }

}
