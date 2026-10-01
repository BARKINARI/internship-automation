namespace WebUI.Dtos.UserDtos;

public class UserWithRolesDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<UserRoleItemDto> Roles { get; set; } = new();
    public int? StajBasvuruId { get; set; }
    public string? Okul { get; set; }
    public string? Bolum { get; set; }
    public string? StajKonusu { get; set; }

    public bool IsAdmin => Roles.Any(r => r.OperationClaimName.Equals("Admin", StringComparison.OrdinalIgnoreCase));
    public bool IsProjeYoneticisi => Roles.Any(r => r.OperationClaimName.Equals("ProjeYoneticisi", StringComparison.OrdinalIgnoreCase));
    public bool IsStajyer => Roles.Any(r => r.OperationClaimName.Equals("Stajyer", StringComparison.OrdinalIgnoreCase));
    public bool IsStajSorumlusu => Roles.Any(r => r.OperationClaimName.Equals("StajSorumlusu", StringComparison.OrdinalIgnoreCase));

    public bool HasStajBasvuruAccess => IsAdmin || Roles.Any(r => r.OperationClaimName.Equals("Staj.Basvurular", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("StajBasvurulari.Read", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("StajSorumlusu", StringComparison.OrdinalIgnoreCase));
    public bool HasYeniBasvuruAccess => IsAdmin || Roles.Any(r => r.OperationClaimName.Equals("Staj.YeniBasvuru", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("StajBasvurulari.Add", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("StajSorumlusu", StringComparison.OrdinalIgnoreCase));
    public bool HasProjeYonetimiAccess => IsAdmin || Roles.Any(r => r.OperationClaimName.Equals("Proje.Yonetimi", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("ProjeYoneticisi", StringComparison.OrdinalIgnoreCase));
    public bool HasProjeTakipAccess => IsAdmin || Roles.Any(r => r.OperationClaimName.Equals("Proje.TakipNotlandirma", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("ProjeYoneticisi", StringComparison.OrdinalIgnoreCase));
    public bool HasMesajAccess => IsAdmin || Roles.Any(r => r.OperationClaimName.Equals("Iletisim.OgrenciMesajlari", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("ProjeYoneticisi", StringComparison.OrdinalIgnoreCase) || r.OperationClaimName.Equals("StajSorumlusu", StringComparison.OrdinalIgnoreCase));
}

public class UserRoleItemDto
{
    public int UserOperationClaimId { get; set; }
    public int OperationClaimId { get; set; }
    public string OperationClaimName { get; set; } = string.Empty;
    public bool IsSystemRole => OperationClaimName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("ProjeYoneticisi", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Stajyer", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("StajSorumlusu", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Egitmen", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Staj.Basvurular", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Staj.YeniBasvuru", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Proje.Yonetimi", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Proje.TakipNotlandirma", StringComparison.OrdinalIgnoreCase) ||
                                OperationClaimName.Equals("Iletisim.OgrenciMesajlari", StringComparison.OrdinalIgnoreCase);

    public bool IsTechnicalClaim => !IsSystemRole;

    public string DisplayName => OperationClaimName switch
    {
        "Admin" => "Super Admin",
        "Staj.Basvurular" => "Staj Basvurulari",
        "Staj.YeniBasvuru" => "Yeni Basvuru Yap",
        "Proje.Yonetimi" => "Proje Yonetimi",
        "Proje.TakipNotlandirma" => "Proje Takip & Not",
        "Iletisim.OgrenciMesajlari" => "Ogrenci Mesajlari",
        "ProjeYoneticisi" => "Proje Yoneticisi",
        "StajSorumlusu" => "Staj Sorumlusu",
        "Stajyer" => "Stajyer",
        "Egitmen" => "Egitmen",
        _ => OperationClaimName
    };

    public string BadgeClass => OperationClaimName.ToLower() switch
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
