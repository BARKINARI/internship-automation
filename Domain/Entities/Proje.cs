using Core.Persistence.Repositories;
using Core.Security.Entities;
using Domain.Enums;

namespace Domain.Entities;

public class Proje : Entity<int>
{
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public ProjeDurumu Durum { get; set; } = ProjeDurumu.Beklemede;

    public int? YoneticiId { get; set; }
    public virtual User<int>? Yonetici { get; set; }

    public virtual ICollection<StajyerProje> StajyerProjeler { get; set; } = new HashSet<StajyerProje>();
    public virtual ICollection<Mesaj> Mesajlar { get; set; } = new HashSet<Mesaj>();

    public Proje() { }

    public Proje(int id) : base(id) { }

    public Proje(int id, string baslik, string aciklama, DateTime baslangicTarihi, DateTime bitisTarihi, ProjeDurumu durum, int? yoneticiId = null)
        : base(id)
    {
        Baslik = baslik;
        Aciklama = aciklama;
        BaslangicTarihi = baslangicTarihi;
        BitisTarihi = bitisTarihi;
        Durum = durum;
        YoneticiId = yoneticiId;
    }
}
