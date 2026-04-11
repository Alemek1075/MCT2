using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MCT.Controllers
{
    public class AuthController : Controller
    {
        private readonly MctContext _context;

        public AuthController(MctContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            TempData["signtempdataname"] = username;
            TempData["signtempdataemail"] = email;
            TempData["signtempdatapass"] = password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["AuthError"] = "register";
                TempData["AuthErrorMessage"] = "All fields are required. Please fill them in.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                TempData["AuthError"] = "register";
                TempData["AuthErrorMessage"] = "Please enter a valid email address.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            string normalizedEmail = email.Trim().ToLower();
            string normalizedUsername = username.Trim().ToLower();

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail || u.Username.ToLower() == normalizedUsername))
            {
                TempData["AuthError"] = "register";
                TempData["AuthErrorMessage"] = "User with this email or username already exists.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var user = new User
            {
                Username = username.Trim(),
                Email = email.Trim(),
                PasswordHash = password,
                Role = "User"
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["AuthError"] = "register";
                TempData["AuthErrorMessage"] = "This username is already taken. Please try another.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string loginIdentifier, string password)
        {
            TempData["logtempdata1"] = loginIdentifier;
            TempData["logtempdatapass"] = password;

            if (string.IsNullOrWhiteSpace(loginIdentifier) || string.IsNullOrWhiteSpace(password))
            {
                TempData["AuthError"] = "login";
                TempData["AuthErrorMessage"] = "Please enter both your username/email and password.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            string normalizedIdentifier = loginIdentifier.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                (u.Email.ToLower() == normalizedIdentifier || u.Username.ToLower() == normalizedIdentifier) &&
                u.PasswordHash == password);

            if (user != null)
            {
                await SignInUser(user);
                return RedirectToAction("Index", "Home");
            }

            TempData["AuthError"] = "login";
            TempData["AuthErrorMessage"] = "Invalid username/email or password.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}