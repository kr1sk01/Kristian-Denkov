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
        Task<IActionResult> ChangePicture(ProfileDto image);
    }
}
