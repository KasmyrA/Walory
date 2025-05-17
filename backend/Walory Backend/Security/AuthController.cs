using Application.DTO;
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

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var confirmationLink = $"{Request.Scheme}://{Request.Host}/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            Console.WriteLine($"Reset link: {confirmationLink}");
            //TU DODAJEMY EMAIL
            return Ok("Registered, check email to confirm");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, isPersistent: true, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized("Invalid login");

            return Ok("Logged in");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest("Invalid token");

            return Ok("Email confirmed");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("User not found or email not confirmed");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var link = $"{Request.Scheme}://{Request.Host}/auth/reset-password?userId={user.Id}&token={encodedToken}";

            Console.WriteLine($"Reset link: {link}");
            //TU DODAJEMY EMAIL
            return Ok("Password reset link generated");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null) return NotFound("User not found");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password has been reset");
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
    }
}
