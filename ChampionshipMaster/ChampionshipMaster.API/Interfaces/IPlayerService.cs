using ChampionshipMaster.SHARED.ViewModels;

namespace ChampionshipMaster.API.Interfaces
{
    public interface IPlayerService
    {
        Task<IActionResult> Register(RegisterViewModel register);
        Task<IActionResult> Login(LoginViewModel login);
    }
}
