using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels;
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

            await memberServices.CreateMemberAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}
