using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IGenericRepository<Plan> _planRepository;
        private readonly IGenericRepository<Membership> _membershipRepository;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;

        public MemberServices(IGenericRepository<Member> memberRepository, IGenericRepository<Plan> planRepository, 
                              IGenericRepository<Membership> membershipRepository, IGenericRepository<HealthRecord> healthRecordRepository,
                              IGenericRepository<Booking> bookingRepository)
        {
            _memberRepository = memberRepository;
            _planRepository = planRepository;
            _membershipRepository = membershipRepository;
            _healthRecordRepository = healthRecordRepository;
            _bookingRepository = bookingRepository;
        }

        //Get

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAll(false, ct);
            if (!members.Any())
                return [];
            var membersViewModel = members.Select(m => new MemberViewModel()
            {
               Id = m.Id,
               Name = m.Name,
               Email = m.Email,
               Phone = m.Phone,
               Photo = m.Photo,
               Gender = m.Gender.ToString()
            });
            return membersViewModel;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetById(memberId, ct);
            if (member == null)
                return null;
            var memberViewModel = new MemberViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}"
            };

            var ActiveMembership = await _membershipRepository.FirstOrDefaultAsync(ms => ms.MemberId == memberId && ms.EndDate > DateTime.Now, false, ct);
            if(ActiveMembership != null)
            {
                var activePlan = await _planRepository.GetById(ActiveMembership.PlanId, ct);

                memberViewModel.PlanName = activePlan?.Name;
                memberViewModel.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }
            return memberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _healthRecordRepository.FirstOrDefaultAsync(hr => hr.MemberId == memberId, false, ct);
            if (record == null)
                return null;
            var healthRecordViewModel = new HealthRecordViewModel()
            {
                Height = record.Height,
                Weight = record.Weight,
                BloodType = record.BloodType,
                Note = record.Note
            };
            return healthRecordViewModel;
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetById(memberId, ct);
            if (member == null)
                return null;
            var memberToUpdateViewModel = new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City,
                Photo = member.Photo
            };
            return memberToUpdateViewModel;
        }

        //Post

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel memberToCreate, CancellationToken ct = default)
        {
            var emailExists = await _memberRepository.AnyAsync(m => m.Email == memberToCreate.Email, ct);
            var phoneExists = await _memberRepository.AnyAsync(m => m.Phone == memberToCreate.Phone, ct);

            if (emailExists || phoneExists)
                return false;

            var member = new Member()
            {
                Name = memberToCreate.Name,
                Email = memberToCreate.Email,
                Phone = memberToCreate.Phone,
                DateOfBirth = memberToCreate.DateOfBirth,
                Gender = memberToCreate.Gender,
                Address = new Address()
                {
                    BuildingNumber = memberToCreate.BuildingNumber,
                    Street = memberToCreate.Street,
                    City = memberToCreate.City
                },
                HealthRecord = new HealthRecord()
                {
                    Height = memberToCreate.HealthRecordViewModel.Height,
                    Weight = memberToCreate.HealthRecordViewModel.Weight,
                    BloodType = memberToCreate.HealthRecordViewModel.BloodType,
                    Note = memberToCreate.HealthRecordViewModel.Note
                }
            };
            _memberRepository.Add(member);

            var result = await _memberRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel memberToUpdate, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetById(memberId, ct);
            if (member == null)
                return false;

            if(await _memberRepository.AnyAsync(m=>m.Email == memberToUpdate.Email && m.Id != memberId, ct)) return false;
            if(await _memberRepository.AnyAsync(m=>m.Phone == memberToUpdate.Phone && m.Id != memberId, ct)) return false;

            member.Email = memberToUpdate.Email;
            member.Phone = memberToUpdate.Phone;
            member.Address.BuildingNumber = memberToUpdate.BuildingNumber;
            member.Address.Street = memberToUpdate.Street;
            member.Address.City = memberToUpdate.City;
            member.UpdatedAt = DateTime.Now;

            _memberRepository.Update(member);

            var result = await _memberRepository.SaveChangesAsync();
            return result > 0;

        }
        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var hasFutureSessions = await _bookingRepository.AnyAsync(b => b.MemberId == memberId && b.Session.EndDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return false;

            _memberRepository.Delete(memberId);

            var result = await _memberRepository.SaveChangesAsync();
            return result > 0; 
        }

    }
}
