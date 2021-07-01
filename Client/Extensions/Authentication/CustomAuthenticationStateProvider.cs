using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Shared;

namespace Client.Extensions.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        /// <summary>
        /// Este método se llama automáticamente por AuhtorizeRouteView en la inicialización
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var storedUser = await _localStorage.GetItemAsync<UserData>("user");

            if (storedUser == null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", storedUser.Token);

            // TODO: Para I18N, establecer el lenguaje preferido contenido en UserData

            return CreateAuthState(storedUser);
        }

        public async Task SetCurrentUserAsync(UserData userData)
        {
            var authState = CreateAuthState(userData);

            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            await _localStorage.SetItemAsync("user", userData);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userData.Token);
        }

        public async Task UpdateUsernameAsync(string newUsername)
        {
            var storedUser = await _localStorage.GetItemAsync<UserData>("user");
            if (storedUser == null)
                return;

            // update username and notify components
            storedUser.Username = newUsername;

            var authState = CreateAuthState(storedUser);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            await _localStorage.SetItemAsync("user", storedUser); //hey, works good!
        }

        public async Task ClearCurrentUserAsync()
        {
            var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var authStateTask = Task.FromResult(new AuthenticationState(anonymousPrincipal));
            NotifyAuthenticationStateChanged(authStateTask);

            await _localStorage.RemoveItemAsync("user");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
        }

        private static AuthenticationState CreateAuthState(UserData userData)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userData.Username)
            };
            claims.AddRange(userData.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Convert back to <Claim>
            foreach (var (key, value) in userData.Policies)
            {
                claims.Add(new Claim(key, value));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "custom");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return new AuthenticationState(claimsPrincipal);
        }
    }
}