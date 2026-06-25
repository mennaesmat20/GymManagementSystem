using GymManagementSystem.BLL.ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMemberServices
    {
        //Get
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default);
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        //Post
        Task<bool> CreateMemberAsync(CreateMemberViewModel memberToCreate, CancellationToken ct = default);
        Task<bool> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel memberToUpdate, CancellationToken ct = default);
        Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default);
    }
}
