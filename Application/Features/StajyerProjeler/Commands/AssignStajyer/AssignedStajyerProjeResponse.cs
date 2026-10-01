using Domain.Enums;

namespace Application.Features.StajyerProjeler.Commands.AssignStajyer;

public class AssignedStajyerProjeResponse
{
    public int Id { get; set; }
    public int ProjeId { get; set; }
    public string ProjeBaslik { get; set; } = string.Empty;
    public int StajBasvuruId { get; set; }
    public string StajyerAdSoyad { get; set; } = string.Empty;
    public DateTime AtanmaTarihi { get; set; }
    public StajyerProjeDurumu Durum { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}