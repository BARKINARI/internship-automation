using WebUI.Controllers;

namespace WebUI.Helpers;

public static class UserPermissionHelper
{
    public const string ClaimAdmin = "Admin";
    public const string ClaimStajBasvurular = "Staj.Basvurular";
    public const string ClaimStajYeniBasvuru = "Staj.YeniBasvuru";
    public const string ClaimProjeYonetimi = "Proje.Yonetimi";
    public const string ClaimProjeTakipNotlandirma = "Proje.TakipNotlandirma";
    public const string ClaimIletisimOgrenciMesajlari = "Iletisim.OgrenciMesajlari";

    public const string RoleProjeYoneticisi = "ProjeYoneticisi";
    public const string RoleStajSorumlusu = "StajSorumlusu";
    public const string RoleStajyer = "Stajyer";

    public static List<string> GetUserRoles(string? token)
    {
        return LoginController.GetUserRolesFromToken(token);
    }

    public static bool HasRole(string? token, string roleName)
    {
        var roles = GetUserRoles(token);
        return roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsAdmin(string? token)
    {
        return HasRole(token, ClaimAdmin);
    }

    public static bool CanAccessBasvurular(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var roles = GetUserRoles(token);
        return roles.Contains(ClaimAdmin, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(ClaimStajBasvurular, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains("StajBasvurulari.Read", StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(RoleStajSorumlusu, StringComparer.OrdinalIgnoreCase);
    }

    public static bool CanAccessYeniBasvuru(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var roles = GetUserRoles(token);
        return roles.Contains(ClaimAdmin, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(ClaimStajYeniBasvuru, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains("StajBasvurulari.Add", StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(RoleStajSorumlusu, StringComparer.OrdinalIgnoreCase);
    }

    public static bool CanAccessProjeYonetimi(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var roles = GetUserRoles(token);
        return roles.Contains(ClaimAdmin, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(ClaimProjeYonetimi, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(RoleProjeYoneticisi, StringComparer.OrdinalIgnoreCase);
    }

    public static bool CanAccessProjeTakip(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var roles = GetUserRoles(token);
        return roles.Contains(ClaimAdmin, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(ClaimProjeTakipNotlandirma, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(RoleProjeYoneticisi, StringComparer.OrdinalIgnoreCase);
    }

    public static bool CanAccessMesajlar(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var roles = GetUserRoles(token);
        return roles.Contains(ClaimAdmin, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(ClaimIletisimOgrenciMesajlari, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(RoleProjeYoneticisi, StringComparer.OrdinalIgnoreCase) ||
               roles.Contains(RoleStajSorumlusu, StringComparer.OrdinalIgnoreCase);
    }

    public static bool CanAccessKullaniciRol(string? token)
    {
        return IsAdmin(token);
    }

    public static bool HasAnyAdminAreaPermission(string? token)
    {
        return CanAccessBasvurular(token) ||
               CanAccessYeniBasvuru(token) ||
               CanAccessProjeYonetimi(token) ||
               CanAccessProjeTakip(token) ||
               CanAccessMesajlar(token) ||
               CanAccessKullaniciRol(token);
    }

    public static string GetDefaultLandingUrl(string? token)
    {
        if (string.IsNullOrEmpty(token)) return "/Login/Index";
        if (CanAccessBasvurular(token)) return "/Admin/AdminStajBasvuru/StajBasvuruList";
        if (CanAccessYeniBasvuru(token)) return "/Admin/AdminStajBasvuru/CreateStajBasvuru";
        if (CanAccessProjeYonetimi(token)) return "/Admin/AdminProje/Index";
        if (CanAccessProjeTakip(token)) return "/Admin/AdminProje/StajyerProjeleri";
        if (CanAccessMesajlar(token)) return "/Admin/AdminMesaj";
        if (CanAccessKullaniciRol(token)) return "/Admin/AdminKullaniciRol/Index";
        return "/Login/Index";
    }
}
