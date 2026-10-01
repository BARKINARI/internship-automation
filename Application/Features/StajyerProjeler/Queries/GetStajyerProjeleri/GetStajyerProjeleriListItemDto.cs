using Domain.Enums;

namespace Application.Features.StajyerProjeler.Queries.GetStajyerProjeleri;

public class GetStajyerProjeleriListItemDto
{
    public int StajyerProjeId { get; set; }
    public int ProjeId { get; set; }
    public string ProjeBaslik { get; set; } = string.Empty;
    public string ProjeAciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public int StajBasvuruId { get; set; }
    public string StajyerAdSoyad { get; set; } = string.Empty;
    public string StajyerEposta { get; set; } = string.Empty;
    public DateTime AtanmaTarihi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
    public StajyerProjeDurumu Durum { get; set; }
    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }
}