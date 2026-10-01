using Core.Persistence.Repositories;
using Core.Security.Entities;
using Domain.Enums;

namespace Domain.Entities;

public class StajyerProje : Entity<int>
{
    public int ProjeId { get; set; }
    public virtual Proje Proje { get; set; } = null!;

    public int StajBasvuruId { get; set; }
    public virtual StajBasvuru StajBasvuru { get; set; } = null!;

    public int? UserId { get; set; }
    public virtual User<int>? User { get; set; }

    public DateTime AtanmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? TeslimTarihi { get; set; }
    public StajyerProjeDurumu Durum { get; set; } = StajyerProjeDurumu.Atandi;

    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }

    public StajyerProje() { }

    public StajyerProje(int id) : base(id) { }

    public StajyerProje(
        int id,
        int projeId,
        int stajBasvuruId,
        int? userId = null,
        DateTime? atanmaTarihi = null,
        StajyerProjeDurumu durum = StajyerProjeDurumu.Atandi
    ) : base(id)
    {
        ProjeId = projeId;
        StajBasvuruId = stajBasvuruId;
        UserId = userId;
        AtanmaTarihi = atanmaTarihi ?? DateTime.UtcNow;
        Durum = durum;
    }
}
