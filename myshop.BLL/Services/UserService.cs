using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL.Models;
using myshop.Entities.Models;

namespace myshop.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedUsersDto> GetUsersAsync(UserQueryDto query)
        {
            var pageNumber = Math.Max(query.PageNumber, 1);
            var pageSize = Math.Max(query.PageSize, 1);

            var usersQuery = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                usersQuery = usersQuery.Where(u =>
                    u.Name.Contains(search) ||
                    (u.Email != null && u.Email.Contains(search)));
            }

            var totalCount = await usersQuery.CountAsync();

            var users = await usersQuery
                .OrderBy(u => u.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<UserDto>(users.Count);
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList(),
                    IsLocked = await _userManager.GetLockoutEndDateAsync(user)
                        > DateTimeOffset.UtcNow
                });
            }

            return new PagedUsersDto
            {
                Users = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = query.Search
            };
        }

        public async Task<bool> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> PromoteToAdminAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var addAdmin = await _userManager.AddToRoleAsync(user, Roles.Admin);
            var removeCustomer = await _userManager.RemoveFromRoleAsync(user, Roles.Customer);

            return addAdmin.Succeeded && removeCustomer.Succeeded;
        }

        public async Task<bool> DemoteToCustomerAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var addCustomer = await _userManager.AddToRoleAsync(user, Roles.Customer);
            var removeAdmin = await _userManager.RemoveFromRoleAsync(user, Roles.Admin);

            return addCustomer.Succeeded && removeAdmin.Succeeded;
        }

        public async Task<bool> LockAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.AddYears(100));

            return result.Succeeded;
        }

        public async Task<bool> UnlockAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            return result.Succeeded;
        }
    }
}
