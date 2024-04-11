﻿using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.SHARED.ViewModels;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            var result = await _playerService.Register(register);
            return result;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var result = await _playerService.Login(login);
            return result;
        }
    }
}
