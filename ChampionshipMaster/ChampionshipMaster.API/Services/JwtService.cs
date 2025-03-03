﻿using ChampionshipMaster.SHARED.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ChampionshipMaster.API.Services
{
    public class JwtService
    {
        private readonly SymmetricSecurityKey _signingKey;
        private readonly UserManager<Player> _userManager;
        private readonly IConfiguration _configuration;

        public JwtService(UserManager<Player> userManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt_Key"]!));
            _userManager = userManager;
            
        }

        public async Task<string> GenerateToken(Player request)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName ?? ""),
                new Claim(ClaimTypes.Email, request.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, request.Id ?? "")
            };

            var roles = await _userManager.GetRolesAsync(request);
            var result = roles.FirstOrDefault();
            if (result != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, result!));
            }
            
            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
