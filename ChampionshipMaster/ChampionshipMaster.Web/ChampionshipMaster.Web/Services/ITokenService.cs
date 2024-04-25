using System.IdentityModel.Tokens.Jwt;

namespace ChampionshipMaster.Web.Services
{
    public interface ITokenService
    {
        Task<bool> ValidateToken(bool requireAdmin = false);
        bool ValidateTokenLifetime(JwtSecurityToken token);
        bool ValidateTokenAdmin(JwtSecurityToken token);
        Task<string> GetToken();
    }
}
