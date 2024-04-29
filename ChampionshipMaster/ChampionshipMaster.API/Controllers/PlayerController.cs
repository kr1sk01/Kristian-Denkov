using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("logout/{username}")]
        public async Task<IActionResult> LogOut(string username)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _playerService.LogOut(username, authHeader);
            return result;
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _playerService.ChangePassword(changePassword, authHeader);
            return result;
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _playerService.ConfirmEmail(userId, token);
            return result;
        }
    }
}
