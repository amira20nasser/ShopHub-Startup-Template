using Microsoft.AspNetCore.Identity;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;
using myshop.DAL.Models;
using myshop.Entities.Models;

namespace myshop.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<RegisterResult> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Name = registerDto.Name,
                Address = string.Empty,
                City = string.Empty
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            if (!await _roleManager.RoleExistsAsync(Roles.Customer))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Customer));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return new RegisterResult
                {
                    Succeeded = false,
                    Errors = roleResult.Errors.Select(e => e.Description).ToList()
                };
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new RegisterResult
            {
                Succeeded = true,
                UserId = user.Id,
                Email = user.Email,
                EmailConfirmationToken = token
            };
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Unable to load the user."
                });
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return IdentityResult.Success;
            }

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<EmailConfirmationData?> GetEmailConfirmationDataAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new EmailConfirmationData
            {
                UserId = user.Id,
                Token = token
            };
        }

        public async Task<SignInResult> LoginAsync(LoginDto loginDto)
        {
            return await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
