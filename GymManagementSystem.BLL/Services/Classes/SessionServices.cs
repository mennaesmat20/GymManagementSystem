using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Session_ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public SessionServices(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken token)
        {
            var sessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(token);
            if (!sessions.Any()) 
                return [];

            sessions = sessions.OrderByDescending(x => x.StartDate);

            var MappedSessions = mapper.Map<IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions);
            foreach(var session in MappedSessions)
            {
                session.AvailableSlots = session.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, token);
            }
            return MappedSessions;
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int SessionId, CancellationToken token)
        {
            var session = await unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategoryAsync(SessionId, token);
            if (session == null) return Result<SessionViewModel>.NotFound("Session not found!");
            var mappedSession = mapper.Map<Session, SessionViewModel>(session);
            mappedSession.AvailableSlots = mappedSession.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(mappedSession.Id, token);
            return Result<SessionViewModel>.Ok(mappedSession);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken token = default)
        {
            var Trainer = await unitOfWork.GetRepository<Trainer>().GetAll(false, token);
            return mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(Trainer);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken token = default)
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAll(false, token);
            return mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken token)
        {
            if (model.EndDate <= model.StartDate) return Result.ValidationFailed("End date must be after start date");
            if (model.StartDate <= DateTime.Now) return Result.ValidationFailed("Start date must be in the future");

            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(model.TrainerId, token);
            if (trainer == null) return Result.NotFound("Trainer not found");
            var category = await unitOfWork.GetRepository<Category>().GetById(model.CategoryId, token);
            if (category == null) return Result.NotFound("Category not found");

            var session = mapper.Map<CreateSessionViewModel, Session>(model);

            var sessionRepo = unitOfWork.GetRepository<Session>();
            sessionRepo.Add(session);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create session");
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken token)
        {
            var session = await unitOfWork.GetRepository<Session>().GetById(sessionId, token);
            if (session == null) 
                return Result<UpdateSessionViewModel>.NotFound("Session not found!");

            if(!await IsSessionVaildToUpdateAsync(session, token)) 
                return Result<UpdateSessionViewModel>.Fail("This session cannot be updated.", ResultStatus.Forbidden);
            var model = mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(model);
        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken token)
        {
            var sessionRepo = unitOfWork.GetRepository<Session>();
            var session =  await sessionRepo.GetById(sessionId, token);
            if (session == null) return Result.NotFound("Session not found");
            if(session.StartDate <= DateTime.Now) return Result.Fail("Cannot update a session that has already started");

            if (model.EndDate <= model.StartDate) return Result.ValidationFailed("End date must be after start date");
            if (model.StartDate <= DateTime.Now) return Result.ValidationFailed("Start date must be in the future");

            var bookedSlotsCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, token);
            if (bookedSlotsCount > 0) return Result.Fail("Cannot update a session that has booked slots");

            var trainer = await unitOfWork.GetRepository<Trainer>().GetById(model.TrainerId, token);
            if (trainer == null) return Result.NotFound("Trainer not found");

            session.UpdatedAt = DateTime.Now;
            mapper.Map(model, session);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update session");
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken token)
        {
            var sessionRepo = unitOfWork.GetRepository<Session>();
            var session = await sessionRepo.GetById(sessionId, token);
            if (session == null) return Result.NotFound("Session Not Found");
            if (session.EndDate >= DateTime.Now) return Result.Fail("Cannot Delete a session that has not yet ended");

            var bookedSlotsCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, token);
            if (bookedSlotsCount > 0) return Result.Fail("Cannot update a session that has booked slots");

            sessionRepo.Delete(sessionId);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update session");
        }

        private async Task<bool> IsSessionVaildToUpdateAsync(Session session, CancellationToken token)
        {
            if(session.StartDate <= DateTime.Now) return false;
            var bookedSlotsCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, token);
            return bookedSlotsCount == 0;
        }

    }
}
