using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.DAL.Entities
{
    public class Membership : BaseEntity
    {
        public DateTime EndDate { get; set; }

        [NotMapped]
        public bool IsActive => EndDate > DateTime.Now;
        public string Status => IsActive ? "Active" : "Expired";

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
    }
}
