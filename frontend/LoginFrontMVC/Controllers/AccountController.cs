using LoginFrontMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace LoginFrontMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5209/api/users/login", model);

            if (response.IsSuccessStatusCode)
            {
                var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, tokenInfo.Username),
                    new Claim("AccessToken", tokenInfo.Token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction(nameof(UsersController.Index), "Users");
            }
            else
            {
                var errors = await response.Content.ReadFromJsonAsync<List<string>>();
                
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5209/api/users", model);

            if (response.IsSuccessStatusCode)
            {
                return View("RegisterSuccess");
            }
            else
            {
                var errors = await response.Content.ReadFromJsonAsync<List<string>>();

                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);                   
                }

                return View(model);
            }       
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5209/api/users/confirm-email/{userId}");

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Email confirmado com sucesso!";
            }
            else
            {
                ViewBag.Message = "Falha ao confirmar o email.";
            }

            return View();
        }
    }
}
