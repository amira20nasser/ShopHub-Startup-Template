using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL.Models;
using myshop.Web.Authorization;
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

        public async Task<IActionResult> Index(UserQueryDto query)
        {
            var pagedUsers = await _userService.GetUsersAsync(query);
            return View(pagedUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (id == GetCurrentUserId())
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.ActiveAccount)]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var promoted = await _userService.PromoteToAdminAsync(id);
            TempData["Update"] = promoted
                ? "User promoted to Admin successfully."
                : "Error while promoting the user.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoteToCustomer(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (id == GetCurrentUserId())
            {
                TempData["Delete"] = "You cannot demote your own account.";
                return RedirectToAction(nameof(Index));
            }

            var demoted = await _userService.DemoteToCustomerAsync(id);
            TempData["Update"] = demoted
                ? "User demoted to Customer successfully."
                : "Error while demoting the user.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (id == GetCurrentUserId())
            {
                TempData["Delete"] = "You cannot lock your own account.";
                return RedirectToAction(nameof(Index));
            }

            var locked = await _userService.LockAsync(id);
            TempData["Update"] = locked
                ? "User locked successfully."
                : "Error while locking the user.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var unlocked = await _userService.UnlockAsync(id);
            TempData["Update"] = unlocked
                ? "User unlocked successfully."
                : "Error while unlocking the user.";

            return RedirectToAction(nameof(Index));
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
