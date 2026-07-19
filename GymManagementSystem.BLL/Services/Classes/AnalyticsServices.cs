using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Analytics_ViewModel;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class AnalyticsServices : IAnalyticsServices
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsServices(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default)
        {
            var sessions = await unitOfWork.GetRepository<Session>().GetAll(false, ct);
            var totalMembers = await unitOfWork.GetRepository<Member>().CountAsync(token: ct);
            var totalTrainers = await unitOfWork.GetRepository<Trainer>().CountAsync(token: ct);
            var activeMembers = await unitOfWork.GetRepository<Membership>().CountAsync(m => m.EndDate > DateTime.Now, ct);

            return new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = sessions.Count(x => x.StartDate > DateTime.Now),
                OngoingSessions = sessions.Count(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now),
                CompletedSessions = sessions.Count(x => x.EndDate < DateTime.Now)
            };
        }
    }
}
