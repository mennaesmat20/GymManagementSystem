using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainer_ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerServices : ITrainerServices
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        public TrainerServices(IGenericRepository<Trainer> trainerRepository)
        {
            _trainerRepository = trainerRepository;
        }

        // Get
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAll(false, ct);
            if (trainers == null)
                return [];
            var trainerViewModels = trainers.Select(t => new TrainerViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialties = t.Specialty.ToString(),
            });
            return trainerViewModels;
        }
            

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetById(trainerId, ct);
            if(trainer == null)
                return null;
            var trainerViewModel = new TrainerViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialties = trainer.Specialty.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            };
            return trainerViewModel;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetById(trainerId, ct);
            if(trainer == null)
                return null;
            var trainerToUpdateViewModel = new TrainerToUpdateViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                Specialties = trainer.Specialty
            };
            return trainerToUpdateViewModel;
        }

        // Post
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel trainerToCreate, CancellationToken ct = default)
        {
            var emailExists = await _trainerRepository.AnyAsync(m => m.Email == trainerToCreate.Email, ct);
            var phoneExists = await _trainerRepository.AnyAsync(m => m.Phone == trainerToCreate.Phone, ct);

            if (emailExists || phoneExists)
                return false;
            var trainer = new Trainer()
            {
                Name = trainerToCreate.Name,
                Email = trainerToCreate.Email,
                Phone = trainerToCreate.Phone,
                DateOfBirth = trainerToCreate.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = trainerToCreate.BuildingNumber,
                    Street = trainerToCreate.Street,
                    City = trainerToCreate.City
                },
                Specialty = trainerToCreate.Specialties
            };
            _trainerRepository.Add(trainer);

            var result = await _trainerRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel trainerToUpdate, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetById(trainerId, ct);
            if (trainer == null)
                return false;
            if (await _trainerRepository.AnyAsync(m => m.Email == trainerToUpdate.Email && m.Id != trainerId, ct)) return false;
            if (await _trainerRepository.AnyAsync(m => m.Phone == trainerToUpdate.Phone && m.Id != trainerId, ct)) return false;

            trainer.Name = trainerToUpdate.Name;
            trainer.Email = trainerToUpdate.Email;
            trainer.Phone = trainerToUpdate.Phone;
            trainer.Address.BuildingNumber = trainerToUpdate.BuildingNumber;
            trainer.Address.Street = trainerToUpdate.Street;
            trainer.Address.City = trainerToUpdate.City;
            trainer.Specialty = trainerToUpdate.Specialties;
            trainer.UpdatedAt = DateTime.Now;

            _trainerRepository.Update(trainer);

            var result = await _trainerRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var HasSessions = await _trainerRepository.AnyAsync(t => t.Id == trainerId && t.Sessions.Any(), ct);
            if (HasSessions)
                return false;

            _trainerRepository.Delete(trainerId);

            var result = await _trainerRepository.SaveChangesAsync();
            return result > 0;
        }
    }
}
