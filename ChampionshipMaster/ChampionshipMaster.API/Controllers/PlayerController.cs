using ChampionshipMaster.API.Interfaces;
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
        public async Task<IActionResult> Regsiter(RegisterViewModel registerViewModel)
        {
            var result = await _playerService.Register(registerViewModel);
            return result;
        }
    }
}
