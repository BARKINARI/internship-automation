using System.ComponentModel.DataAnnotations;

namespace WebUI.Dtos.UserDtos;

public class ResultOperationClaimDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public bool IsSystemRole => Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                Name.Equals("ProjeYoneticisi", StringComparison.OrdinalIgnoreCase) ||
                                Name.Equals("Stajyer", StringComparison.OrdinalIgnoreCase) ||
                                Name.Equals("StajSorumlusu", StringComparison.OrdinalIgnoreCase) ||
                                Name.Equals("Egitmen", StringComparison.OrdinalIgnoreCase) ||
                                IsModularMenuClaim;

    public bool IsTechnicalClaim => !IsSystemRole;

    public bool IsModularMenuClaim =>
        Name.Equals("Staj.Basvurular", StringComparison.OrdinalIgnoreCase) ||
        Name.Equals("Staj.YeniBasvuru", StringComparison.OrdinalIgnoreCase) ||
        Name.Equals("Proje.Yonetimi", StringComparison.OrdinalIgnoreCase) ||
        Name.Equals("Proje.TakipNotlandirma", StringComparison.OrdinalIgnoreCase) ||
        Name.Equals("Iletisim.OgrenciMesajlari", StringComparison.OrdinalIgnoreCase) ||
        Name.Equals("Admin", StringComparison.OrdinalIgnoreCase);

    public string DisplayName => Name switch
    {
        "Admin" => "Tam Yetkili Yonetici (Admin)",
        "Staj.Basvurular" => "Staj Basvurulari",
        "Staj.YeniBasvuru" => "Yeni Basvuru Yap",
        "Proje.Yonetimi" => "Proje Yonetimi",
        "Proje.TakipNotlandirma" => "Proje Takip & Notlandirma",
        "Iletisim.OgrenciMesajlari" => "Ogrenci Mesajlari",
        "ProjeYoneticisi" => "Proje Yoneticisi",
        "StajSorumlusu" => "Staj Sorumlusu",
        "Stajyer" => "Stajyer Ogrenci",
        "Egitmen" => "Egitmen",
        _ => Name
    };

    public string Category => Name switch
    {
        "Staj.Basvurular" or "Staj.YeniBasvuru" or "StajSorumlusu" => "STAJ BASVURU ISLEMLERI",
        "Proje.Yonetimi" or "Proje.TakipNotlandirma" or "ProjeYoneticisi" => "PROJE VE GOREV ISLEMLERI",
        "Iletisim.OgrenciMesajlari" => "ILETISIM",
        "Admin" => "SISTEM & YETKILENDIRME",
        _ => "DIGER / TEKNIK YETKILER"
    };

    public string Icon => Name switch
    {
        "Admin" => "fas fa-user-shield",
        "Staj.Basvurular" => "fas fa-clipboard-list",
        "Staj.YeniBasvuru" => "fas fa-plus-circle",
        "Proje.Yonetimi" => "fas fa-project-diagram",
        "Proje.TakipNotlandirma" => "fas fa-award",
        "Iletisim.OgrenciMesajlari" => "fas fa-comments",
        "ProjeYoneticisi" => "fas fa-user-tie",
        "StajSorumlusu" => "fas fa-user-check",
        _ => "fas fa-shield-alt"
    };

    public string Description => Name switch
    {
        "Admin" => "Tum menulere, basvurulara, projelere ve kullanici/rol atamalarina tam erisim",
        "Staj.Basvurular" => "Staj basvurularini listeleme, detay inceleme, basvuru onaylama ve reddetme",
        "Staj.YeniBasvuru" => "Yonetici adina sisteme yeni staj basvurusu ekleme formu",
        "Proje.Yonetimi" => "Yeni proje tanimlama, proje duzenleme ve listeleme",
        "Proje.TakipNotlandirma" => "Stajyerlere proje gorevlendirme ve stajyer projelerini notlandirma",
        "Iletisim.OgrenciMesajlari" => "Kabul edilen stajyer ogrencilerle anlik cift tarafli mesajlasma",
        "ProjeYoneticisi" => "Proje yonetimi, gorev takibi ve stajyer mesajlasma yetkileri paketi",
        "StajSorumlusu" => "Staj basvuru surecleri ve ogrenci iletisimi yetkileri paketi",
        _ => "Sistem teknik islem yetkisi"
    };

    public string BadgeClass => Name.ToLower() switch
    {
        "admin" => "badge-danger",
        "staj.basvurular" => "badge-primary",
        "staj.yenibasvuru" => "badge-info",
        "proje.yonetimi" => "badge-primary",
        "proje.takipnotlandirma" => "badge-warning",
        "iletisim.ogrencimesajlari" => "badge-success",
        "projeyoneticisi" => "badge-primary",
        "stajyer" => "badge-secondary",
        "stajsorumlusu" => "badge-success",
        "egitmen" => "badge-warning",
        _ => "badge-light border text-dark"
    };
}

public class ResultOperationClaimPaginationDto
{
    public List<ResultOperationClaimDto> Items { get; set; } = new();
    public int Count { get; set; }
}
