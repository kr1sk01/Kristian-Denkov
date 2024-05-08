﻿using ChampionshipMaster.API.Interfaces;
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

        public PlayerService(UserManager<Player> userManager, JwtService jwtService, IEmailSender emailSender, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _context = context;
        }
        public async Task<List<PlayerDto>> GetAllActivePlayers()
        {
            var activePlayers = await _userManager.Users.Where(x=>x.Active == true).ToArrayAsync();
            var players = activePlayers.Adapt<List<PlayerDto>>();
            return players;
        }
        public async Task<IActionResult> Register(RegisterViewModel registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var player = new Player { 
                UserName = registerRequest.UserName, 
                Email = registerRequest.Email,
                CreatedBy = registerRequest.CreatedBy,
                CreatedOn = registerRequest.CreatedOn,
            };
            var result = await _userManager.CreateAsync(player, registerRequest.Password!);
            await _userManager.AddToRoleAsync(player, "user");

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(registerRequest.Email!);
                var team = new Team()
                {
                    Name = user.UserName,
                    Active = true,
                    CreatedBy = user.UserName,
                    CreatedOn = DateTime.UtcNow,
                    TeamType = await _context.TeamTypes.FirstOrDefaultAsync(x => x.Name!.ToLower() == "solo"),
                    Logo = user.Avatar
                };

                await _context.Teams.AddAsync(team);
                await _context.SaveChangesAsync();

                var teamPlayers = new TeamPlayers()
                {
                    Player = user,
                    Team = team,
                    CreatedBy = user.UserName,
                    CreatedOn = DateTime.UtcNow
                };

                user.TeamPlayers.Add(teamPlayers);
                await _context.SaveChangesAsync();

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
                return Redirect("https://localhost:56665/login");
            }

            return BadRequest(result.Errors);
        }

        public async Task<IActionResult> ChangeAvatar(ProfileDto request, StringValues authHeader)
        {
            if(request.Avatar == null || request.Avatar.Length == 0)
            {
                return BadRequest("Image not received");
            }

            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userName = token.Claims.First(c => c.Type == "unique_name").Value;

                var user = await _userManager.FindByNameAsync(userName) ?? throw new Exception($"Unable to find user - {userName}");

                user.Avatar = request.Avatar;
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
                await _context.SaveChangesAsync();

                var newToken = await _jwtService.GenerateToken(user);

                user!.Online = true;

                await _context.SaveChangesAsync();

                string imageString;
                if (user.Avatar.IsNullOrEmpty()) { imageString = ""; }
                else { imageString = Convert.ToBase64String(user.Avatar!); }

                return Ok(new { message = "Login successful!", jwtToken = newToken, image = imageString });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }
    }
}
