using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Member_ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberServices memberServices;
        public MemberController(IMemberServices _memberServices)
        {
            memberServices = _memberServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await memberServices.GetAllMembersAsync(ct);
            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(nameof(Create), model);

            var result = await memberServices.CreateMemberAsync(model, ct);
            if(result)
                TempData["Success"] = "Member created successfully.";
            else
                TempData["Failed"] = "Failed to create member.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await memberServices.GetMemberDetailsAsync(id, ct);
            if(member is null)
            {
                TempData["Failed"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await memberServices.GetMemberHealthRecordAsync(id, ct);
            if(healthRecord is null)
            {
                TempData["Failed"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await memberServices.GetMemberToUpdateAsync(id, ct);
            if(member is null)
            {
                TempData["Failed"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id,MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await memberServices.UpdateMemberDetailsAsync(id, model, ct);
            if(result)
                TempData["Success"] = "Member updated successfully.";
            else
                TempData["Failed"] = "Failed to update member.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await memberServices.GetMemberDetailsAsync(id, ct);
            if(member is null)
            {
                TempData["Failed"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct)
        {
            var result =  await memberServices.DeleteMemberAsync(id, ct);
            if(result)
                TempData["Success"] = "Member deleted successfully.";
            else
                TempData["Failed"] = "Failed to delete member. The member may have future sessions.";

            return RedirectToAction(nameof(Index));
        }

    }
}
