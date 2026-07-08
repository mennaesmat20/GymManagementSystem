using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Plan_ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanServices : IPlanServices
    {
        private readonly IUnitOfWork unitOfWork;
        public PlanServices(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAll(false,ct);
            if(!plans.Any())
                return [];

            var PlanViewModels = plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Duration = p.DurationDays,
                Description = p.Description,
                IsActive = p.IsActive
            });
            return PlanViewModels;
        }

        public async Task<PlanViewModel?> GetPlanDetailsAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
                return null;

            var PlanViewModel = new PlanViewModel
            {
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };
            return PlanViewModel;
        }

        public async Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
                return null;

            var PlanToUpdateViewModel = new PlanToUpdateViewModel
            {
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price
            };
            return PlanToUpdateViewModel;
        }

        public async Task<bool> UpdatePlanDetailsAsync(int planId, PlanToUpdateViewModel planToUpdate, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
                return false;

            if(plan.Name != planToUpdate.PlanName)
                return false;

            plan.Description = planToUpdate.Description;
            plan.DurationDays = planToUpdate.DurationDays;
            plan.Price = planToUpdate.Price;

            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.CompleteAsync();
            return result > 0;

        }

        public async Task<bool> ToggleChangePlanStatusAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null) return false;

            var hasMemberShip = await unitOfWork.GetRepository<Plan>().AnyAsync(p => p.Id == planId && p.Memberships.Any(), ct);
            if (hasMemberShip) return false;

            if (plan.IsActive)
                plan.IsActive = false;
            else
                plan.IsActive = true;

            var result = await unitOfWork.CompleteAsync();
            return result > 0;
        }
    }
}
