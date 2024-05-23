using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.Extensions.Primitives;

namespace ChampionshipMaster.API.Interfaces
{
    public interface IPlayerService
    {
        Task<IActionResult> Register(RegisterViewModel register);
        Task<IActionResult> Login(LoginViewModel login);
        Task<IActionResult> LogOut(string username, StringValues authHeader);
        Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword, StringValues authHeader);
        Task<IActionResult> ConfirmEmail(string userId, string token);
        Task<IActionResult> ChangeAvatar(string newAvatar, StringValues authHeader);
        Task<IActionResult> ChangeUsername(string newUsername, StringValues authHeader);
        Task<List<PlayerDto>> GetAllActivePlayers();
        Task<ActionResult<List<string?>>> GetPlayerUsernamesById(List<string> playerIds);
    }
}
