using WebUI.Dtos.StajBasvuruDtos;

namespace WebUI.Dtos.ProjeDtos;

public class AdminProjeViewModel
{
    public List<ResultProjeDto> Projeler { get; set; } = new();
    public List<ResultStajBasvuruDto> KabulEdilenOgrenciler { get; set; } = new();
}
