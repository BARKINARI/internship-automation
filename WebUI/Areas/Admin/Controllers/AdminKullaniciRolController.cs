using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebUI.Controllers;
using WebUI.Dtos.UserDtos;
using WebUI.Helpers;

namespace WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminKullaniciRolController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public AdminKullaniciRolController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7125";
    }

    private HttpClient CreateApiClient(out string? token)
    {
        var client = _httpClientFactory.CreateClient();
        token = Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    private bool CheckAdminAccess()
    {
        var token = Request.Cookies["access_token"];
        return UserPermissionHelper.IsAdmin(token);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lütfen önce sisteme giriş yapınız.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!CheckAdminAccess())
        {
            TempData["ErrorMessage"] = "Kullanıcı ve Rol Yönetimi sayfasına yalnızca 'Admin' yetkisine sahip kullanıcılar erişebilir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        List<UserWithRolesDto> users = new();
        List<ResultOperationClaimDto> allRoles = new();

        try
        {
            var usersResponse = await client.GetAsync($"{_baseUrl}/api/Users/GetListWithRoles");
            if (usersResponse.IsSuccessStatusCode)
            {
                var json = await usersResponse.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<UserWithRolesDto>>(json) ?? new List<UserWithRolesDto>();
            }

            var rolesResponse = await client.GetAsync($"{_baseUrl}/api/OperationClaims?PageIndex=0&PageSize=200");
            if (rolesResponse.IsSuccessStatusCode)
            {
                var json = await rolesResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultOperationClaimPaginationDto>(json);
                if (result?.Items != null)
                {
                    allRoles = result.Items
                        .Where(r => r.IsModularMenuClaim)
                        .OrderBy(r => r.Id)
                        .ToList();
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Veriler yüklenirken hata oluştu: {ex.Message}";
        }

        ViewBag.AllRoles = allRoles;
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRoles(SetUserRolesDto dto)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token) || !CheckAdminAccess())
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        dto.OperationClaimIds ??= new List<int>();

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/api/Users/SetRoles", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Kullanıcı rolleri başarıyla güncellendi.";
        }
        else
        {
            var err = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Roller güncellenirken bir hata oluştu: {err}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserWithRolesDto dto)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token) || !CheckAdminAccess())
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları eksiksiz doldurunuz.";
            return RedirectToAction(nameof(Index));
        }

        dto.OperationClaimIds ??= new List<int>();

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/api/Users/CreateWithRoles", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = $"'{dto.FirstName} {dto.LastName}' kullanıcısı başarıyla oluşturuldu ve yetkilendirildi.";
        }
        else
        {
            var err = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Kullanıcı oluşturulamadı: {err}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token) || !CheckAdminAccess())
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        var response = await client.PutAsync($"{_baseUrl}/api/Users/ToggleStatus/{id}", null);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Kullanıcının aktiflik durumu değiştirildi.";
        }
        else
        {
            TempData["ErrorMessage"] = "Kullanıcı durumu güncellenemedi.";
        }

        return RedirectToAction(nameof(Index));
    }
}
