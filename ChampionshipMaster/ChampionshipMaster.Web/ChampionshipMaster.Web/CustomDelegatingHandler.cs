using ChampionshipMaster.Web.Services;
using System.Net.Http.Headers;

namespace ChampionshipMaster.Web
{
    public class CustomDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService; // Service to retrieve JWT token

        public CustomDelegatingHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var token = await _tokenService.GetToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            catch { }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
