using GymManagementSystem.DAL.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.DAL.Entities
{
    public class Trainer : GymUser
    {
        public Specialty Specialty { get; set; }
        //public DateTime HireDate { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
