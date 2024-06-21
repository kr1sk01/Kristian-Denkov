using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.SHARED.DTO;
using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
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
        private readonly IConfiguration _configuration;

        public PlayerService(UserManager<Player> userManager, JwtService jwtService, IEmailSender emailSender, ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _context = context;
            _configuration = configuration;
        }
        public async Task<List<PlayerDto>> GetAllActivePlayers()
        {
            var activePlayers = await _userManager.Users.Where(x => x.Active == true).ToArrayAsync();
            var players = activePlayers.Adapt<List<PlayerDto>>();
            return players;
        }
        public async Task<IActionResult> Register(RegisterViewModel registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var player = new Player
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                CreatedOn = registerRequest.CreatedOn
            };
            var result = await _userManager.CreateAsync(player, registerRequest.Password!);
            await _userManager.AddToRoleAsync(player, "user");

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(registerRequest.Email!);
                if (user == null)
                {
                    return BadRequest(new[]
                    {
                        new { name = "Database error", description = "Something went wrong" }
                    });
                }

                user.CreatedBy = user.Id;
                user.Active = true;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
                var userId = user.Id;

                string apiAdress;
                if (_configuration["CustomEnvironment"] == "Development")
                {
                    apiAdress = "https://localhost:50397";
                }
                else
                {
                    apiAdress = "http://10.244.44.38:8091";
                }

                var confirmationLink = $"{apiAdress}/api/Player/confirmEmail?userId=" +
                    Uri.EscapeDataString($"{userId}") +
                    "&token=" +
                    Uri.EscapeDataString($"{emailToken}");

                var task = Task.Run(async () =>
                {
                    await _emailSender.SendAccountConfirmationEmail(user.Email!, user.UserName!, confirmationLink);
                });

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
            var token = await _jwtService.GenerateToken(user);

            user!.Online = true;

            await _context.SaveChangesAsync();

            string imageString;
            if (user.Avatar.IsNullOrEmpty()) { imageString = ""; }
            else { imageString = Convert.ToBase64String(user.Avatar!); }

            return Ok(new { message = "Login successful", jwtToken = token, image = imageString });
        }

        public async Task<IActionResult> LogOut(string username, StringValues authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || string.IsNullOrEmpty(username))
            {
                return BadRequest("Missing authorization");
            }

            var userToLogOut = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (userToLogOut == null)
            {
                return BadRequest("User not found!");

            }
            userToLogOut.Online = false;

            await _context.SaveChangesAsync();

            return Ok("User logged out successfully!");
        }

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword, StringValues authHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid form data!");
            }

            if (string.IsNullOrEmpty(authHeader) || authHeader.ToString() == "Bearer ")
            {
                return BadRequest("Missing authorization!");
            }

            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userName = token.Claims.First(c => c.Type == "unique_name").Value;

                var user = await _userManager.FindByNameAsync(userName) ?? throw new Exception($"Unable to find user - {userName}");

                if (await _userManager.CheckPasswordAsync(user, changePassword.Password!))
                {
                    var result = await _userManager.ChangePasswordAsync(user, changePassword.Password!, changePassword.NewPassword!);

                    if (result.Succeeded)
                    {
                        user.ModifiedOn = DateTime.UtcNow;
                        user.ModifiedBy = user.UserName;

                        await _context.SaveChangesAsync();
                        return Ok("Password changed successfully!");
                    }

                    return BadRequest("Failed to change password!");
                }

                return BadRequest("Current password is invalid!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong");
            }
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
                user.Active = true;
                await _context.SaveChangesAsync();
                return Redirect((_configuration["CustomEnvironment"] == "Development") ? "https://localhost:56665/login" : "http://10.244.44.38:8090/login");
            }

            return BadRequest(result.Errors);
        }

        public async Task<IActionResult> ChangeAvatar(string newAvatar, StringValues authHeader)
        {
            if (string.IsNullOrEmpty(newAvatar))
            {
                return BadRequest("Image not received");
            }

            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userName = token.Claims.First(c => c.Type == "unique_name").Value;

                var user = await _userManager.FindByNameAsync(userName) ?? throw new Exception($"Unable to find user - {userName}");

                user.Avatar = Convert.FromBase64String(newAvatar);
                user.ModifiedBy = userName;
                user.ModifiedOn = DateTime.UtcNow;
                _context.Entry(user).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong");
            }

            return Ok("Avatar updated successfully");
        }

        public async Task<IActionResult> ChangeUsername(string newUsername, StringValues authHeader)
        {
            try
            {
                if (await _userManager.FindByNameAsync(newUsername) != null)
                {
                    return BadRequest("The username is already taken!");
                }

                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userName = token.Claims.First(c => c.Type == "unique_name").Value;

                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    return BadRequest("Wrong credentials. Please try to log in again");
                }

                user.UserName = newUsername;
                user.NormalizedUserName = newUsername.ToUpper();
                await _context.SaveChangesAsync();

                var newToken = await _jwtService.GenerateToken(user);

                user!.Online = true;

                await _context.SaveChangesAsync();

                string imageString;
                if (user.Avatar.IsNullOrEmpty()) { imageString = ""; }
                else { imageString = Convert.ToBase64String(user.Avatar!); }

                return Ok(new { message = "Username changed successfully!", jwtToken = newToken, image = imageString });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }

        public async Task<ActionResult<List<string?>>> GetPlayerUsernamesById(List<string> playerIds)
        {
            var players = new List<Player?>();
            foreach (var playerId in playerIds)
            {
                players.Add(await _context.Users.FirstOrDefaultAsync(x => x.Id == playerId));
            }
            var usernames = new List<string?>();

            foreach (var player in players)
            {
                if (player == null)
                {
                    usernames.Add(null);
                }
                else
                {
                    usernames.Add(player.UserName);
                }
            }
            return Ok(usernames);
        }

        public async Task<IActionResult> EditPlayer(string playerId, PlayerDto player, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var playerToEdit = await _userManager.FindByIdAsync(playerId);

                if (playerToEdit == null)
                {
                    return NotFound("This player doesn't exist!");
                }

                if (playerToEdit.Id != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                if (player.Name != playerToEdit.UserName && player.Name != null)
                {
                    if (await _userManager.FindByNameAsync(player.Name) != null)
                    {
                        return BadRequest("The username is already taken!");
                    }

                    playerToEdit.UserName = player.Name;
                    playerToEdit.NormalizedUserName = player.Name.ToUpper();
                }

                if (player.Avatar != null)
                {
                    playerToEdit.Avatar = player.Avatar;
                }

                if (player.Active != null)
                {
                    playerToEdit.Active = player.Active;
                }

                playerToEdit.ModifiedBy = userId;
                playerToEdit.ModifiedOn = DateTime.UtcNow;

                _context.Entry(playerToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var newToken = await _jwtService.GenerateToken(playerToEdit);

                string imageString;
                if (playerToEdit.Avatar.IsNullOrEmpty()) { imageString = ""; }
                else { imageString = Convert.ToBase64String(playerToEdit.Avatar!); }

                return Ok(new { message = "Login successful!", jwtToken = newToken, image = imageString });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong");
            }
        }
    }
}
