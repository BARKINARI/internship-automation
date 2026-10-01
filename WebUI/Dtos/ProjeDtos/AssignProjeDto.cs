using System.ComponentModel.DataAnnotations;

namespace WebUI.Dtos.ProjeDtos;

public class AssignProjeDto
{
    [Required]
    public int ProjeId { get; set; }

    [Required]
    public int StajBasvuruId { get; set; }
}
