using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebUI.Dtos.AuthDtos;

using WebUI.Helpers;

namespace WebUI.Controllers;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public LoginController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7125";
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (Request.Cookies.ContainsKey("access_token"))
        {
            var token = Request.Cookies["access_token"];
            if (UserPermissionHelper.HasAnyAdminAreaPermission(token))
            {
                return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
            }
            if (Request.Cookies.ContainsKey("ogrenci_basvuru_id"))
            {
                return RedirectToAction("Mesajlar", "Ogrenci");
            }
        }
        return View(new UserLoginDto());
    }

    [HttpPost]
    public async Task<IActionResult> Index(UserLoginDto userLoginDto)
    {
        if (!ModelState.IsValid)
            return View(userLoginDto);

        try
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(new
            {
                email = userLoginDto.Email,
                password = userLoginDto.Password
            });
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_baseUrl}/api/Auth/Login", stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(responseContent);

                if (tokenDto != null && !string.IsNullOrEmpty(tokenDto.Token))
                {
                    var cookieExpires = tokenDto.ExpirationDate > DateTime.UtcNow
                        ? tokenDto.ExpirationDate
                        : DateTime.UtcNow.AddMinutes(60);

                    Response.Cookies.Append("access_token", tokenDto.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = cookieExpires,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        Path = "/"
                    });

                    dynamic? loginObj = JsonConvert.DeserializeObject(responseContent);
                    string? fName = (string?)(loginObj?.firstName ?? loginObj?.FirstName);
                    string? lName = (string?)(loginObj?.lastName ?? loginObj?.LastName);
                    string fullName = $"{fName} {lName}".Trim();
                    if (!string.IsNullOrWhiteSpace(fullName))
                    {
                        Response.Cookies.Append("user_fullname", fullName, new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = cookieExpires,
                            Secure = Request.IsHttps,
                            SameSite = SameSiteMode.Lax,
                            Path = "/"
                        });
                    }

                    if (UserPermissionHelper.HasAnyAdminAreaPermission(tokenDto.Token))
                    {
                        return Redirect(UserPermissionHelper.GetDefaultLandingUrl(tokenDto.Token));
                    }
                    else
                    {
                        var basvuruResponse = await client.GetAsync($"{_baseUrl}/api/Mesajlar/GetBasvuruByEmail/{Uri.EscapeDataString(userLoginDto.Email)}");
                        if (basvuruResponse.IsSuccessStatusCode)
                        {
                            var basvuruJson = await basvuruResponse.Content.ReadAsStringAsync();
                            dynamic? basvuruObj = JsonConvert.DeserializeObject(basvuruJson);
                            if (basvuruObj != null)
                            {
                                int durum = (int)(basvuruObj.stajBasvuruDurumu ?? basvuruObj.StajBasvuruDurumu ?? 0);
                                int basvuruId = (int)(basvuruObj.id ?? basvuruObj.Id ?? 0);
                                string adSoyad = (string)(basvuruObj.adSoyad ?? basvuruObj.AdSoyad ?? "Öğrenci");
                                string? konu = (string?)(basvuruObj.stajKonusu ?? basvuruObj.StajKonusu ?? "");

                                if (durum == 2 && basvuruId > 0)
                                {
                                    var ogrenciCookieOptions = new CookieOptions
                                    {
                                        HttpOnly = true,
                                        Expires = cookieExpires,
                                        Secure = Request.IsHttps,
                                        SameSite = SameSiteMode.Lax,
                                        Path = "/"
                                    };

                                    Response.Cookies.Append("ogrenci_basvuru_id", basvuruId.ToString(), ogrenciCookieOptions);
                                    Response.Cookies.Append("ogrenci_ad_soyad", adSoyad, ogrenciCookieOptions);
                                    Response.Cookies.Append("ogrenci_konu", konu ?? "", ogrenciCookieOptions);

                                    int? tokenUserId = GetUserIdFromToken(tokenDto.Token) ?? (int?)(basvuruObj.userId ?? basvuruObj.UserId);
                                    if (tokenUserId.HasValue)
                                    {
                                        Response.Cookies.Append("ogrenci_user_id", tokenUserId.Value.ToString(), ogrenciCookieOptions);
                                    }

                                    return RedirectToAction("Mesajlar", "Ogrenci");
                                }
                                else
                                {
                                    Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
                                    ViewBag.Error = "Staj başvurunuz henüz onaylanmamış veya değerlendirme aşamasındadır.";
                                    return View(userLoginDto);
                                }
                            }
                        }

                        Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
                        ViewBag.Error = "Onaylanmış bir staj başvurusu kaydı bulunamadı. Lütfen yöneticiniz ile iletişime geçiniz.";
                        return View(userLoginDto);
                    }
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                dynamic? errObj = JsonConvert.DeserializeObject(errorContent);
                if (errObj != null)
                {
                    string? rawMsg = null;
                    if (errObj.Errors != null && errObj.Errors.Count > 0)
                    {
                        var firstErrGroup = errObj.Errors[0];
                        if (firstErrGroup.Errors != null && firstErrGroup.Errors.Count > 0)
                        {
                            rawMsg = firstErrGroup.Errors[0].ToString();
                        }
                    }
                    if (string.IsNullOrWhiteSpace(rawMsg) && errObj.detail != null)
                    {
                        rawMsg = errObj.detail.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(rawMsg))
                    {
                        string friendlyMsg = rawMsg
                            .Replace("'User For Login Dto Password'", "Sifre")
                            .Replace("UserForLoginDto.Password", "Sifre")
                            .Replace("'User For Login Dto Email'", "E-posta")
                            .Replace("UserForLoginDto.Email", "E-posta")
                            .Replace("User For Login Dto Password", "Sifre")
                            .Replace("User For Login Dto Email", "E-posta");

                        ViewBag.Error = friendlyMsg;
                        return View(userLoginDto);
                    }
                }
            }
            catch { }

            ViewBag.Error = "E-posta veya şifre hatalı! Lütfen bilgilerinizi kontrol ediniz.";
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Error = $"WebAPI sunucusuna ({_baseUrl}) bağlanılamadı. Lütfen WebAPI servisinin çalıştığından emin olunuz. ({ex.Message})";
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Giriş yapılırken bir hata oluştu: {ex.Message}";
        }

        return View(userLoginDto);
    }

    [HttpGet]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("user_fullname", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_basvuru_id", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_user_id", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_ad_soyad", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_konu", new CookieOptions { Path = "/" });
        return RedirectToAction("Index");
    }

    public static int? GetUserIdFromToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return null;
        try
        {
            var parts = token.Split('.');
            if (parts.Length > 1)
            {
                string payload = parts[1];
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                dynamic? claimsObj = JsonConvert.DeserializeObject(json);
                if (claimsObj != null)
                {
                    string? idStr = (string?)(claimsObj["nameid"] ?? claimsObj["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);
                    if (int.TryParse(idStr, out int uid)) return uid;
                }
            }
        }
        catch { }
        return null;
    }

    public static List<string> GetUserRolesFromToken(string? token)
    {
        var roles = new List<string>();
        if (string.IsNullOrEmpty(token)) return roles;

        try
        {
            var parts = token.Split('.');
            if (parts.Length > 1)
            {
                string payload = parts[1];
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                string[] roleKeys = ["role", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
                foreach (var key in roleKeys)
                {
                    if (root.TryGetProperty(key, out var roleProp))
                    {
                        if (roleProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var item in roleProp.EnumerateArray())
                            {
                                if (item.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    string? val = item.GetString();
                                    if (!string.IsNullOrWhiteSpace(val)) roles.Add(val.Trim());
                                }
                            }
                        }
                        else if (roleProp.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            string? val = roleProp.GetString();
                            if (!string.IsNullOrWhiteSpace(val)) roles.Add(val.Trim());
                        }
                    }
                }
            }
        }
        catch { }

        return roles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public static string GetUserNameFromToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return "Kullanıcı";

        try
        {
            var parts = token.Split('.');
            if (parts.Length > 1)
            {
                string payload = parts[1];
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                string[] nameKeys = [
                    "unique_name",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                    "name"
                ];

                foreach (var key in nameKeys)
                {
                    if (root.TryGetProperty(key, out var nameProp) && nameProp.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var val = nameProp.GetString();
                        if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
                    }
                }

                string[] emailKeys = ["email", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
                foreach (var key in emailKeys)
                {
                    if (root.TryGetProperty(key, out var emailProp) && emailProp.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var email = emailProp.GetString();
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            var atIndex = email.IndexOf('@');
                            return atIndex > 0 ? email.Substring(0, atIndex) : email;
                        }
                    }
                }
            }
        }
        catch { }

        return "Kullanıcı";
    }

    public static bool IsUserAdmin(string? token)
    {
        var roles = GetUserRolesFromToken(token);
        return roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
    }
}