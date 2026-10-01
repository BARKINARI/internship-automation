namespace WebUI.Dtos.StajBasvuruDtos;

public class ResultStajBasvuruPaginationDto
{
    public List<ResultStajBasvuruDto> Items { get; set; } = [];
    public int Index { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}