using GymManagementSystem.BLL.ViewModels.Plan_ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        //Get
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsAsync(int planId, CancellationToken ct = default);
        Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);

        //Post
        Task<bool> UpdatePlanDetailsAsync(int planId, PlanToUpdateViewModel planToUpdate, CancellationToken ct = default);
        Task<bool> ToggleChangePlanStatusAsync(int planId, CancellationToken ct = default);
    }
}
