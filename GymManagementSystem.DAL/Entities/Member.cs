namespace GymManagementSystem.DAL.Entities
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        //public DateTime JoinDate { get; set; }

        public HealthRecord? HealthRecord { get; set; }

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
