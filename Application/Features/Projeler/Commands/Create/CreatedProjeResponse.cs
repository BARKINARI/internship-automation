using Domain.Enums;

namespace Application.Features.Projeler.Commands.Create;

public class CreatedProjeResponse
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public ProjeDurumu Durum { get; set; }
    public int? YoneticiId { get; set; }
    public DateTime CreatedDate { get; set; }
}
