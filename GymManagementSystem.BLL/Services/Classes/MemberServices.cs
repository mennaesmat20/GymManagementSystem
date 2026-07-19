using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Member_ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAttachmentServices attachmentServices;

        public MemberServices(IUnitOfWork _unitOfWork, IMapper _mapper, IAttachmentServices _attachmentServices)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            attachmentServices = _attachmentServices;
        }

        //Get

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAll(false, ct);
            if (!members.Any())
                return [];

            var membersViewModel = mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return membersViewModel;
        }

        public async Task<Result<MemberViewModel>> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member == null)
                return Result<MemberViewModel>.NotFound("Member not found!");

            var memberViewModel = mapper.Map<Member,MemberViewModel>(member);

            var ActiveMembership = await unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(ms => ms.MemberId == memberId && ms.EndDate > DateTime.Now, false, ct);
            if(ActiveMembership != null)
            {
                var activePlan = await unitOfWork.GetRepository<Plan>().GetById(ActiveMembership.PlanId, ct);

                memberViewModel.PlanName = activePlan?.Name;
                memberViewModel.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }
            return Result<MemberViewModel>.Ok(memberViewModel);
        }

        public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(hr => hr.MemberId == memberId, false, ct);
            if (record == null)
                return Result<HealthRecordViewModel>.NotFound("Health record not found!");

            var healthRecordViewModel = mapper.Map<HealthRecord, HealthRecordViewModel>(record);
            return Result<HealthRecordViewModel>.Ok(healthRecordViewModel);
        }

        public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member == null)
                return Result<MemberToUpdateViewModel>.NotFound("Member not found!");

            var memberToUpdateViewModel = mapper.Map<Member, MemberToUpdateViewModel>(member);
            return Result<MemberToUpdateViewModel>.Ok(memberToUpdateViewModel);
        }

        //Post

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel memberToCreate, CancellationToken ct = default)
        {
            var emailExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == memberToCreate.Email, ct);
            var phoneExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == memberToCreate.Phone, ct);

            if (emailExists)
                return Result.ValidationFailed("This email is already exist");
            else if (phoneExists)
                return Result.ValidationFailed("This phone number is already exist");

            //var NewPhotoName = await attachmentServices.UploadAsync(memberToCreate.PhotoFile.OpenReadStream(), memberToCreate.PhotoFile.FileName, "MembersPictures", ct);
            //if (string.IsNullOrEmpty(NewPhotoName))
            //    return Result.ValidationFailed("Profile photo upload failed (check file type and size).");
            string? NewPhotoName = null;

            if (memberToCreate.PhotoFile != null)
            {
                NewPhotoName = await attachmentServices.UploadAsync(
                    memberToCreate.PhotoFile.OpenReadStream(),
                    memberToCreate.PhotoFile.FileName,
                    "MembersPictures",
                    ct
                );

                if (string.IsNullOrEmpty(NewPhotoName))
                    return Result.ValidationFailed("Profile photo upload failed (check file type and size).");
            }

            var member = mapper.Map<CreateMemberViewModel, Member>(memberToCreate);
            member.Photo = NewPhotoName;

            unitOfWork.GetRepository<Member>().Add(member);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create a member");
        }

        public async Task<Result> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel memberToUpdate, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member == null)
                return Result.NotFound("Member not found!");

            if (await unitOfWork.GetRepository<Member>().AnyAsync(m=>m.Email == memberToUpdate.Email && m.Id != memberId, ct)) 
                return Result.ValidationFailed("This email is already exist");
            if (await unitOfWork.GetRepository<Member>().AnyAsync(m=>m.Phone == memberToUpdate.Phone && m.Id != memberId, ct)) 
                return Result.ValidationFailed("This phone number is already exist");

            mapper.Map(memberToUpdate, member);
            member.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Member>().Update(member);

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update a member");
        }
        public async Task<Result> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var memberRepo = unitOfWork.GetRepository<Member>();
            var member = await memberRepo.GetById(memberId, ct);
            if (member is null) return Result.NotFound("Member not found.");

            var hasFutureSessions = await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.EndDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return Result.ValidationFailed("Can not delete a member with future sessions");

            memberRepo.Delete(memberId);
            if(member.Photo is not null)
            {
                attachmentServices.Delete(member.Photo, "MembersPictures");
            }
            

            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete a member");
        }

    }
}
