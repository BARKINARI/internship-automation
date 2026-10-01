using Domain.Enums;

namespace Application.Features.Projeler.Queries.GetList;

public class GetListProjeListItemDto
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public ProjeDurumu Durum { get; set; }
    public int? YoneticiId { get; set; }
    public string? YoneticiAd { get; set; }
    public int AtananStajyerSayisi { get; set; }
    public DateTime CreatedDate { get; set; }
}
