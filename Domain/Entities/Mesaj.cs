using Core.Persistence.Repositories;
using Core.Security.Entities;

namespace Domain.Entities;

public class Mesaj : Entity<int>
{
    public int GonderenId { get; set; }
    public virtual User<int> GonderenUser { get; set; } = null!;

    public int AliciId { get; set; }
    public virtual User<int> AliciUser { get; set; } = null!;

    public int? StajBasvuruId { get; set; }
    public virtual StajBasvuru? StajBasvuru { get; set; }

    public int? ProjeId { get; set; }
    public virtual Proje? Proje { get; set; }

    public string Icerik { get; set; } = string.Empty;
    public string? DosyaYolu { get; set; }
    public string? DosyaAdi { get; set; }
    public bool OkunduMu { get; set; } = false;

    public Mesaj() { }

    public Mesaj(int id) : base(id) { }

    public Mesaj(
        int id,
        int gonderenId,
        int aliciId,
        string icerik,
        bool okunduMu = false,
        int? stajBasvuruId = null,
        int? projeId = null,
        string? dosyaYolu = null,
        string? dosyaAdi = null
    ) : base(id)
    {
        GonderenId = gonderenId;
        AliciId = aliciId;
        Icerik = icerik;
        OkunduMu = okunduMu;
        StajBasvuruId = stajBasvuruId;
        ProjeId = projeId;
        DosyaYolu = dosyaYolu;
        DosyaAdi = dosyaAdi;
    }
}
