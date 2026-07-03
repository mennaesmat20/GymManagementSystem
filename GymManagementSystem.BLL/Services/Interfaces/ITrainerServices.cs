using GymManagementSystem.BLL.ViewModels.Trainer_ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ITrainerServices
    {
        //Get
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId , CancellationToken ct = default);

        //Post
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel trainerToCreate, CancellationToken ct = default);
        Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel trainerToUpdate, CancellationToken ct = default);
        Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct = default);
    }
}
