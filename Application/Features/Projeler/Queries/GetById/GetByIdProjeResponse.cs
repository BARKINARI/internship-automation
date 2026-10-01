using Domain.Enums;

namespace Application.Features.Projeler.Queries.GetById;

public class GetByIdProjeResponse
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public ProjeDurumu Durum { get; set; }
    public int? YoneticiId { get; set; }
    public string? YoneticiAd { get; set; }
    public List<ProjeStajyerItemDto> Stajyerler { get; set; } = new();
    public DateTime CreatedDate { get; set; }
}

public class ProjeStajyerItemDto
{
    public int StajyerProjeId { get; set; }
    public int StajBasvuruId { get; set; }
    public string StajyerAdSoyad { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public DateTime AtanmaTarihi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
    public StajyerProjeDurumu Durum { get; set; }
    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }
}
