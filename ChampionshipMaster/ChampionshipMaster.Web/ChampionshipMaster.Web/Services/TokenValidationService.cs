
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace ChampionshipMaster.Web.Services
{
    public class TokenValidationService : ITokenValidation
    {
        private readonly ProtectedLocalStorage _localStorage;

        public TokenValidationService(ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<bool> ValidateToken(bool requireAdmin = false)
        {
            try
            {
                var result = await _localStorage.GetAsync<string>("jwtToken");

                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    var tokenString = result.Value;

                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(tokenString);

                    List<bool> validatorsList = new List<bool>();

                    validatorsList.Add(ValidateTokenLifetime(token));

                    if (requireAdmin == true) 
                    {
                        validatorsList.Add(ValidateTokenAdmin(token));
                    }

                    return !validatorsList.Any(x => x == false);
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _localStorage.DeleteAsync("jwtToken");
                return false;
            }
        }

        public bool ValidateTokenAdmin(JwtSecurityToken token)
        {
            var role = token.Claims.FirstOrDefault(x => x.Type == "role");
            if (role == null) { return false; }
            return role.Value == "admin";
        }

        public bool ValidateTokenLifetime(JwtSecurityToken token)
        {
            var expiryTimeUnix = token.Payload.Expiration;

            if (expiryTimeUnix == null)
            {
                return false;
            }

            var expiryTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expiryTimeUnix!.Value).DateTime;
            if (expiryTimeUtc < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
    }
}
