using GymManagementSystem.BLL.ViewModels.Analytics_ViewModel;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IAnalyticsServices
    {
        public Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default);
    }
}
