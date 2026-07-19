using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Plan_ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanServices : IPlanServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PlanServices(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAll(false,ct);
            if(!plans.Any())
                return [];

            var PlanViewModels = mapper.Map<IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);
            return PlanViewModels;
        }

        public async Task<Result<PlanViewModel>> GetPlanDetailsAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
                return Result<PlanViewModel>.NotFound("Plan not found!");

            var PlanViewModel = mapper.Map<Plan,PlanViewModel>(plan);
            return Result<PlanViewModel>.Ok(PlanViewModel);
        }

        public async Task<Result<PlanToUpdateViewModel>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
                return Result<PlanToUpdateViewModel>.NotFound("Plan not found!");

            var PlanToUpdateViewModel = mapper.Map<Plan, PlanToUpdateViewModel>(plan);
            return Result<PlanToUpdateViewModel>.Ok(PlanToUpdateViewModel);
        }

        public async Task<Result> UpdatePlanDetailsAsync(int planId, PlanToUpdateViewModel planToUpdate, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null)
                return Result.NotFound("Plan not found!");

            if(plan.Name != planToUpdate.PlanName)
                return Result.Fail("Can not change the plan name!");

            mapper.Map(planToUpdate, plan);

            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update the plan");

        }

        public async Task<Result> ToggleChangePlanStatusAsync(int planId, CancellationToken ct = default)
        {
            var plan = await unitOfWork.GetRepository<Plan>().GetById(planId, ct);
            if (plan == null) return Result.NotFound("Plan not found!");

            var hasMemberShip = await unitOfWork.GetRepository<Plan>().AnyAsync(p => p.Id == planId && p.Memberships.Any(), ct);
            if (hasMemberShip) return Result.Fail("Can not change the plan while it is in an membership");

            if (plan.IsActive)
                plan.IsActive = false;
            else
                plan.IsActive = true;

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete the plan");
        }
    }
}
