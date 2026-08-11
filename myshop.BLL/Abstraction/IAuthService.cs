using Microsoft.AspNetCore.Identity;
using myshop.BLL.DTOs;

namespace myshop.BLL.Abstraction
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(RegisterDto registerDto);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<EmailConfirmationData?> GetEmailConfirmationDataAsync(string email);
        Task<SignInResult> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
    }
}
