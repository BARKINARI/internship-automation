using Core.Persistence.Repositories;
using Core.Security.Entities;
using Domain.Enums;

namespace Domain.Entities;

public class StajBasvuru : Entity<int>
{
    public int? UserId { get; set; }
    public virtual User<int>? User { get; set; }

    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string Okul { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Sinif { get; set; } = string.Empty;
    public DateTime StajBaslangicTarihi { get; set; }
    public DateTime StajBitisTarihi { get; set; }
    public string? Referans { get; set; }
    public string? KendiniTanitmaYazisi { get; set; }
    public string? CvDosyaYolu { get; set; }
    public string? StajKonusu { get; set; }
    public virtual StajBasvuruDurumu StajBasvuruDurumu { get; set; } = StajBasvuruDurumu.Beklemede;

    public virtual ICollection<Mesaj> Mesajlar { get; set; } = new HashSet<Mesaj>();
    public virtual ICollection<StajyerProje> StajyerProjeler { get; set; } = new HashSet<StajyerProje>();

    public StajBasvuru() { }

    public StajBasvuru(int id) : base(id) { }
}