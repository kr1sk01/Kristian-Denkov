using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class PlayerService : ControllerBase, IPlayerService
    {
        private readonly UserManager<Player> _userManager;
        private readonly JwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public PlayerService(UserManager<Player> userManager, JwtService jwtService, IEmailSender emailSender, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Register(RegisterViewModel registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Player { 
                UserName = registerRequest.UserName, 
                Email = registerRequest.Email,
                CreatedBy = registerRequest.CreatedBy,
                CreatedOn = registerRequest.CreatedOn,
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password!);
            await _userManager.AddToRoleAsync(user, "user");

            if (result.Succeeded)
            {
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var userId = user.Id;
                var confirmationLink = $"https://localhost:50397/api/Player/confirmEmail?userId=" +
                    Uri.EscapeDataString($"{userId}") +
                    "&token=" +
                    Uri.EscapeDataString($"{emailToken}");
                await _emailSender.SendAccountConfirmationEmail(user.Email!, user.UserName!, confirmationLink);

                var jwtToken = _jwtService.GenerateToken(user);
                return Ok(new { message = "Registration successful", jwtToken = jwtToken.Result });
            }

            return BadRequest(result.Errors);
        }

        public async Task<IActionResult> Login(LoginViewModel loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by username
            var user = await _userManager.FindByEmailAsync(loginRequest.Email!);

            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            // Validate the password
            var validPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password!);

            if (!validPassword)
            {
                return BadRequest("Invalid username or password");
            }

            // Generate a JWT token on successful loginRequest
            var token = _jwtService.GenerateToken(user);

            var activeUser = await _context.Users.FirstOrDefaultAsync(x=>x.NormalizedEmail == loginRequest.Email!.ToUpper());

            activeUser!.Active = true;

            _context.SaveChanges();

            return Ok(new { message = "Login successful", jwtToken = token.Result });
        }

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword, StringValues authHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(authHeader))
            {
                return BadRequest("Missing authorization");
            }

            var tokenString = authHeader.ToString().Split(' ')[1];
            var token = new JwtSecurityToken(tokenString);

            var userName = token.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = await _userManager.FindByNameAsync(userName!);

            var result = await _userManager.ChangePasswordAsync(user!, changePassword.Password, changePassword.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }

            return BadRequest("Something went wrong");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid confirmation link.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Redirect("https://localhost:56665/login");
            }

            return BadRequest(result.Errors);
        }
    }
}
