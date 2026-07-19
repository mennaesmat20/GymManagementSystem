using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Member_ViewModels;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMemberServices
    {
        //Get
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task<Result<MemberViewModel>> GetMemberDetailsAsync(int memberId, CancellationToken ct = default);
        Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default);
        Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        //Post
        Task<Result> CreateMemberAsync(CreateMemberViewModel memberToCreate, CancellationToken ct = default);
        Task<Result> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel memberToUpdate, CancellationToken ct = default);
        Task<Result> DeleteMemberAsync(int memberId, CancellationToken ct = default);
    }
}
