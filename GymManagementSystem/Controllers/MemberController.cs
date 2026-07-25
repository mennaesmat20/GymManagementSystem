using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Member_ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberServices memberServices;
        private readonly IAttachmentServices attachmentServices;

        public MemberController(IMemberServices _memberServices, IAttachmentServices _attachmentServices)
        {
            memberServices = _memberServices;
            attachmentServices = _attachmentServices;
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
            if(result.Success)
                TempData["Success"] = "Member created successfully.";
            else
                TempData["Failed"] = "Failed to create member.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await memberServices.GetMemberDetailsAsync(id);
            if (member.Value is null || string.IsNullOrWhiteSpace(member.Value.Photo))
                return NotFound();


            var result = attachmentServices.GetFile(member.Value.Photo, "MembersPictures");
            if (result is null) return NotFound();

            return File(result.Value.Stream, result.Value.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var result = await memberServices.GetMemberDetailsAsync(id, ct);
            if(!result.Success)
            {
                TempData["Failed"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await memberServices.GetMemberHealthRecordAsync(id, ct);
            if(!result.Success)
            {
                TempData["Failed"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await memberServices.GetMemberToUpdateAsync(id, ct);
            if(!result.Success)
            {
                TempData["Failed"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id,MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await memberServices.UpdateMemberDetailsAsync(id, model, ct);
            if(result.Success)
                TempData["Success"] = "Member updated successfully.";
            else
                TempData["Failed"] = "Failed to update member.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await memberServices.GetMemberDetailsAsync(id, ct);
            if(!result.Success)
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
            if(result.Success)
                TempData["Success"] = "Member deleted successfully.";
            else
                TempData["Failed"] = "Failed to delete member. The member may have future sessions.";

            return RedirectToAction(nameof(Index));
        }

    }
}
