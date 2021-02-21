using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Shared;

namespace Client.Extensions.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public CustomAuthenticationStateProvider(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Este método se llama automáticamente por AuhtorizeRouteView en la inicialización
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var storedUser = await _jsRuntime.InvokeAsync<UserData>("localStorage.getItem", "user");

            if (storedUser == null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, storedUser.Username)
            };

            claims.AddRange(storedUser.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(storedUser.Policies);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", storedUser.Token);

            // TODO: Para I18N, establecer el lenguaje preferido contenido en UserData

            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "custom")));
        }

        public async Task SetCurrentUserAsync(UserData userData)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userData.Username)
            };
            claims.AddRange(userData.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(userData.Policies);

            var claimsIdentity = new ClaimsIdentity(claims, "custom");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authState = new AuthenticationState(claimsPrincipal);

            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", userData);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userData.Token);
        }

        public async Task ClearCurrentUserAsync()
        {
            var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var authStateTask = Task.FromResult(new AuthenticationState(anonymousPrincipal));
            NotifyAuthenticationStateChanged(authStateTask);

            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}