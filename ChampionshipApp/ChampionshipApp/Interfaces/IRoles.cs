using Microsoft.AspNetCore.Mvc;

namespace ChampionshipApp.Interfaces;

public interface IRoles
{
   Task<string> AddRole(string[] roles);

}
