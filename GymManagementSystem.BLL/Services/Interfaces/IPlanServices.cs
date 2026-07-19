using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Plan_ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        //Get
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<Result<PlanViewModel>> GetPlanDetailsAsync(int planId, CancellationToken ct = default);
        Task<Result<PlanToUpdateViewModel>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);

        //Post
        Task<Result> UpdatePlanDetailsAsync(int planId, PlanToUpdateViewModel planToUpdate, CancellationToken ct = default);
        Task<Result> ToggleChangePlanStatusAsync(int planId, CancellationToken ct = default);
    }
}
