﻿using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.Extensions.Primitives;

namespace ChampionshipMaster.API.Interfaces
{
    public interface IPlayerService
    {
        Task<IActionResult> Register(RegisterViewModel register);
        Task<IActionResult> Login(LoginViewModel login);
        Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword, StringValues authHeader);
    }
}
