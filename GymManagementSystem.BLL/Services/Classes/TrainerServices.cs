using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainer_ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerServices : ITrainerServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TrainerServices(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        // Get
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await unitOfWork.GetRepository<Trainer>().GetAll(false, ct);
            if (trainers == null)
                return [];
            var trainerViewModels = mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerViewModel>>(trainers);
            return trainerViewModels;
        }
            

        public async Task<Result<TrainerViewModel>> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(trainerId, ct);
            if(trainer == null)
                return Result<TrainerViewModel>.NotFound("Trainer not found!");
            var trainerViewModel = mapper.Map<Trainer,TrainerViewModel>(trainer);
            return Result<TrainerViewModel>.Ok(trainerViewModel);
        }

        public async Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(trainerId, ct);
            if(trainer == null)
                return Result<TrainerToUpdateViewModel>.NotFound("Trainer not found!");
            var trainerToUpdateViewModel = mapper.Map<Trainer, TrainerToUpdateViewModel>(trainer);
            return Result<TrainerToUpdateViewModel>.Ok(trainerToUpdateViewModel);
        }

        // Post
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel trainerToCreate, CancellationToken ct = default)
        {
            var emailExists = await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == trainerToCreate.Email, ct);
            var phoneExists = await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == trainerToCreate.Phone, ct);

            if (emailExists)
                return Result.ValidationFailed("This email is already exist");
            else if (phoneExists)
                return Result.ValidationFailed("This phone number is already exist");

            var trainer = mapper.Map<CreateTrainerViewModel,Trainer>(trainerToCreate);
            unitOfWork.GetRepository<Trainer>().Add(trainer);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create a trainer");
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel trainerToUpdate, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(trainerId, ct);
            if (trainer == null)
                return Result.NotFound("Trainer not found!");

            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == trainerToUpdate.Email && m.Id != trainerId, ct))
                return Result.ValidationFailed("This email is already exist");
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == trainerToUpdate.Phone && m.Id != trainerId, ct))
                return Result.ValidationFailed("This phone number is already exist");

            mapper.Map(trainerToUpdate, trainer);
            trainer.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Trainer>().Update(trainer);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update a trainer");
        }

        public async Task<Result> DeleteTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var HasSessions = await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Id == trainerId && t.Sessions.Any(), ct);
            if (HasSessions)
                return Result.ValidationFailed("Can not delete a trainer with future sessions");

            unitOfWork.GetRepository<Trainer>().Delete(trainerId);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete a trainer");
        }
    }
}
