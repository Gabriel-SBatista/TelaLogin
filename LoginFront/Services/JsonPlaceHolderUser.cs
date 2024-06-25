using LoginFront.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace LoginFront.Services
{
    public class JsonPlaceHolderUser
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        public JsonPlaceHolderUser(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public async Task<List<User>> GetUsers()
        {
            using var response = await _httpClient.GetAsync("api/users");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("notauthorized");
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<User>>();

            return result;
        }
    }
}
