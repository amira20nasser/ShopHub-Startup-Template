using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Abstraction;
using myshop.DAL.Models;
using System.Security.Claims;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsers();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
            {
                TempData["Delete"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var deleted = await _userService.Delete(id);
            if (!deleted)
            {
                TempData["Delete"] = "Error while deleting the user.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Delete"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
