using Blazored.LocalStorage;
using LoginFront.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace LoginFront.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }
        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var messageResult = await _httpClient.PostAsJsonAsync("api/users", registerModel);
            var result = await messageResult.Content.ReadFromJsonAsync<RegisterResult>();
            return result;
        }
        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/login", loginModel);
                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var loginResultError = new LoginResult { Errors = JsonSerializer.Deserialize<List<string>>(await response.Content.ReadAsStringAsync()) };
                        return loginResultError;
                    }
                    catch
                    {
                        return new LoginResult { Errors = new List<string> { "teste" } };
                    }
                }
                var loginResult = JsonSerializer.Deserialize<LoginResult>(await
                    response.Content.ReadAsStringAsync(), new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true });
                await _localStorage.SetItemAsync("authToken", loginResult.Token);
                ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                    .MarkUserAsAuthenticated(loginModel.Username);
                _httpClient.DefaultRequestHeaders.Authorization = new
                    AuthenticationHeaderValue("bearer", loginResult.Token);
                return loginResult;
            }
            catch (Exception)
            {
                return new LoginResult { Errors = new List<string> { "catch" } };
            }
            
        }
        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
