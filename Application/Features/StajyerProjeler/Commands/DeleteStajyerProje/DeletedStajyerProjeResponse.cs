namespace Application.Features.StajyerProjeler.Commands.DeleteStajyerProje;

public class DeletedStajyerProjeResponse
{
    public int Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}