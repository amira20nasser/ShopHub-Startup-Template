using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
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

        public async Task<IReadOnlyList<UserDto>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserDto>(users.Count);
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return result;
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
    }
}
