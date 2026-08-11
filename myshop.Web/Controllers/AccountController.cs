using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Abstraction;
using myshop.BLL.DTOs;

namespace myshop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            IAuthService authService,
            IEmailService emailService,
            IWebHostEnvironment environment)
        {
            _authService = authService;
            _emailService = emailService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(registerDto);
            }

            await SendEmailConfirmationAsync(result.Email!, result.UserId!, result.EmailConfirmationToken!);

            TempData["Success"] = "Registration successful. Please confirm your email.";
            return RedirectToAction(nameof(RegisterConfirmation), new { email = result.Email });
        }

        [HttpGet]
        public async Task<IActionResult> RegisterConfirmation(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            string? confirmationLink = null;

            if (_environment.IsDevelopment())
            {
                var data = await _authService.GetEmailConfirmationDataAsync(email);
                if (data != null)
                {
                    confirmationLink = Url.Action(
                        nameof(ConfirmEmail),
                        "Account",
                        new { userId = data.UserId, token = data.Token },
                        Request.Scheme);
                }
            }

            ViewData["Email"] = email;
            ViewData["ConfirmationLink"] = confirmationLink;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var result = await _authService.ConfirmEmailAsync(userId, token);
            ViewData["Success"] = result.Succeeded;
            ViewData["StatusMessage"] = result.Succeeded
                ? "Thank you for confirming your email."
                : "Error confirming your email.";

            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginDto { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(loginDto.ReturnUrl) && Url.IsLocalUrl(loginDto.ReturnUrl))
                {
                    return LocalRedirect(loginDto.ReturnUrl);
                }

                return RedirectToAction("Index", "Product");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "You must confirm your email before you can log in. Please check your inbox for the confirmation link.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(loginDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SendEmailConfirmationAsync(string email, string userId, string token)
        {
            var callbackUrl = Url.Action(
                nameof(ConfirmEmail),
                "Account",
                new { userId, token },
                Request.Scheme);

            await _emailService.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
        }
    }
}
