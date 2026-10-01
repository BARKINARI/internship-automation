using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebUI.Controllers;

namespace WebUI.ViewComponents.AdminLayoutViewComponents;

public class _AdminLayoutTopbarComponentPartial : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public _AdminLayoutTopbarComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7125";
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string? fullName = HttpContext.Request.Cookies["user_fullname"];
        if (string.IsNullOrWhiteSpace(fullName))
        {
            string? token = HttpContext.Request.Cookies["access_token"];
            int? userId = LoginController.GetUserIdFromToken(token);
            if (userId.HasValue)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var userRes = await client.GetAsync($"{_baseUrl}/api/Users/{userId.Value}");
                    if (userRes.IsSuccessStatusCode)
                    {
                        var userJson = await userRes.Content.ReadAsStringAsync();
                        dynamic? uObj = JsonConvert.DeserializeObject(userJson);
                        string? f = (string?)(uObj?.firstName ?? uObj?.FirstName);
                        string? l = (string?)(uObj?.lastName ?? uObj?.LastName);
                        fullName = $"{f} {l}".Trim();
                    }
                }
                catch { }
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = LoginController.GetUserNameFromToken(token);
            }
        }

        ViewBag.UserName = fullName;
        return View();
    }
}