using Domain.Enums;

namespace Application.Features.StajyerProjeler.Queries.GetList;

public class GetListStajyerProjeListItemDto
{
    public int Id { get; set; }
    public int ProjeId { get; set; }
    public string ProjeBaslik { get; set; } = string.Empty;
    public int StajBasvuruId { get; set; }
    public string StajyerAdSoyad { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public DateTime AtanmaTarihi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
    public StajyerProjeDurumu Durum { get; set; }
    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }
}