using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Session_ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ISessionServices
    {
        public Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken token);
        public Task<SessionViewModel?> GetSessionByIdAsync(int SessionId, CancellationToken token);
        public Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken token = default);
        public Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken token = default);
        public Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken token);

        public Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken token);
        public Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken token);
        public Task<Result> RemoveSessionAsync(int sessionId, CancellationToken token);
    }
}
