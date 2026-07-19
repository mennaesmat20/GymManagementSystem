using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Trainer_ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ITrainerServices
    {
        //Get
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<Result<TrainerViewModel>> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default);
        Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int trainerId , CancellationToken ct = default);

        //Post
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel trainerToCreate, CancellationToken ct = default);
        Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel trainerToUpdate, CancellationToken ct = default);
        Task<Result> DeleteTrainerAsync(int trainerId, CancellationToken ct = default);
    }
}
