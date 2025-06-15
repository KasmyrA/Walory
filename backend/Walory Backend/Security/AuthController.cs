using Application.DTO;
using Application.Services;
using Domain;
using Infrastracture;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Security.Claims;

namespace Walory_Backend.Security
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Email,
                Name = dto.Name,
                EmailConfirmed = false,
                EmailConfirmationCode = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var confirmationLink = $"{Request.Scheme}://localhost:5173/confirm-email?userId={user.Id}&code={user.EmailConfirmationCode}";

            await _emailService.SendEmailAsync(dto.Email, "Confirm your email", confirmationLink);

            return Ok("Registered successfully. Please check your email to confirm your account.");
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid login");

            if (!user.EmailConfirmed)
                return Unauthorized("Email not confirmed");

            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized("Invalid password");

            return Ok("Logged in");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out");
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found");

            if (user.EmailConfirmed)
                return BadRequest("Email already confirmed");

            if (user.EmailConfirmationCode != code)
                return BadRequest("Invalid confirmation code");

            user.EmailConfirmed = true;
            user.EmailConfirmationCode = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to confirm email");

            return Ok("Email confirmed successfully");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
                return BadRequest("User not found or email not confirmed");

            user.PasswordResetCode = Guid.NewGuid().ToString();
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1); // ważny 1h

            await _userManager.UpdateAsync(user);

            var resetLink = $"{Request.Scheme}://localhost:5173/auth/reset-password?userId={user.Id}&code={user.PasswordResetCode}";

            await _emailService.SendEmailAsync(email, "Reset Password", resetLink);

            return Ok("Password reset link sent to your email.");
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                return NotFound("User not found");

            if (user.PasswordResetCode != dto.Code)
                return BadRequest("Invalid reset code");
            if (user.PasswordResetExpiry < DateTime.UtcNow)
                return BadRequest("Expired reset code");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Wyczyść token resetu po udanym restarcie
            user.PasswordResetCode = null;
            user.PasswordResetExpiry = null;
            await _userManager.UpdateAsync(user);

            return Ok("Password has been reset successfully.");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password changed");
        }

        [Authorize]
        [HttpPost("request-email-change")]
        public async Task<IActionResult> RequestEmailChange([FromBody] string newEmail)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            user.EmailChangeCode = Guid.NewGuid().ToString();
            user.PendingNewEmail = newEmail;
            await _userManager.UpdateAsync(user);

            var confirmationLink = $"{Request.Scheme}://localhost:5173/auth/confirm-email-change?userId={user.Id}&token={user.EmailChangeCode}";

            await _emailService.SendEmailAsync(newEmail, "Confirm email change", confirmationLink);
            return Ok("Link sent");
        }

        [HttpGet("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromQuery] Guid userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return BadRequest("User not found");

            if (user.EmailChangeCode != token)
                return BadRequest("Invalid token or email");

            user.Email = user.PendingNewEmail;
            user.UserName = user.PendingNewEmail;
            user.EmailChangeCode = null;
            user.PendingNewEmail = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest("Could not update email");

            return Ok("Email changed");
        }
    }
}
