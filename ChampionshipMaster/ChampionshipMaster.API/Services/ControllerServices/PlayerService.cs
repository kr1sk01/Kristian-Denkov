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

        public PlayerService(UserManager<Player> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<IActionResult> Register(RegisterViewModel registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Player { UserName = registerRequest.UserName, Email = registerRequest.Email };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(new { message = "Registration successful", jwtToken = token.Result });
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
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            // Validate the password
            var validPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!validPassword)
            {
                return BadRequest("Invalid username or password");
            }

            // Generate a JWT token on successful loginRequest
            var token = _jwtService.GenerateToken(user);

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
    }
}
