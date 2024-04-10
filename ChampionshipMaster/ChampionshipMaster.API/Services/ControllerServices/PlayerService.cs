﻿using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class PlayerService : ControllerBase, IPlayerService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Player> _userManager;
        private readonly SignInManager<Player> _signInManager;

        public PlayerService(ApplicationDbContext context, UserManager<Player> userManager, SignInManager<Player> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Player { UserName = registerViewModel.Email, Email = registerViewModel.Email };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(new { message = "Registration successful" });
            }

            return BadRequest(result.Errors);
        }
    }
}
