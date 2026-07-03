
namespace GymManagementSystem.BLL.ViewModels.Plan_ViewModels
{
    public class PlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
