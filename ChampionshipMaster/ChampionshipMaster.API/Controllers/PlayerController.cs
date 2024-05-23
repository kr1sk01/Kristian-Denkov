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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllActivePlayers()
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _playerService.GetAllActivePlayers();
            return Ok(result);
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

        [Authorize]
        [HttpPost("changeAvatar")]
        public async Task<IActionResult> ChangeAvatar([FromBody] Dictionary<string, string> content)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var newAvatar = content.First().Value;
            var result = await _playerService.ChangeAvatar(newAvatar, authHeader);
            return result;
        }

        [Authorize]
        [HttpPost("changeUsername")]
        public async Task<IActionResult> ChangeUsername([FromBody] Dictionary<string, string> content)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var newUsername = content.First().Value;
            var result = await _playerService.ChangeUsername(newUsername, authHeader);
            return result;
        }

        [Authorize]
        [HttpPost("getPlayersById")]
        public async Task<ActionResult<List<string?>>> GetPlayerUsernamesById([FromBody] List<string> content)
        {
            var result = await _playerService.GetPlayerUsernamesById(content);
            return result;
        }
    }
}
