using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using myshop.Entities.Models;
using System.Security.Claims;

namespace myshop.Web.Authorization
{
    public class ActiveAccountAuthorizationHandler : AuthorizationHandler<ActiveAccountRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ActiveAccountAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ActiveAccountRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
            if (lockoutEnd.HasValue && lockoutEnd > DateTimeOffset.UtcNow)
            {
                return;
            }

            context.Succeed(requirement);
        }
    }
}
