using Microsoft.AspNetCore.Identity;
using static System.Formats.Asn1.AsnWriter;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using ChampionshipApp.Interfaces;

namespace ChampionshipApp.Services;

public class Roles : IRoles
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public Roles(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<string> AddRole(string[] roles)
    {

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                return "Roles Created!";
            }
                


            
        }

        return "Roles already excisting";
    }

}
