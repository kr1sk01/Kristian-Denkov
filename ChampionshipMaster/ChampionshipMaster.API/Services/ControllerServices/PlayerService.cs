using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class PlayerService : ControllerBase, IPlayerService
    {
        private readonly UserManager<Player> _userManager;
        private readonly SignInManager<Player> _signInManager;
        private readonly JwtService _jwtService;

        public PlayerService(UserManager<Player> userManager, SignInManager<Player> signInManager, JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Player { UserName = registerViewModel.UserName, Email = registerViewModel.Email };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                var token = _jwtService.GenerateToken(registerViewModel);
                return Ok(new { message = "Registration successful", jwtToken = token });
            }

            return BadRequest(result.Errors);
        }
    }
}
