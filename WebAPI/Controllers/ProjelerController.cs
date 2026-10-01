using Application.Features.Projeler.Commands.Create;
using Application.Features.Projeler.Commands.Delete;
using Application.Features.Projeler.Commands.Update;
using Application.Features.Projeler.Queries.GetById;
using Application.Features.Projeler.Queries.GetList;
using Application.Features.StajyerProjeler.Commands.AssignStajyer;
using Application.Features.StajyerProjeler.Commands.UpdateStajyerProje;
using Application.Features.StajyerProjeler.Queries.GetStajyerProjeleri;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjelerController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateProjeCommand command)
    {
        CreatedProjeResponse result = await Mediator.Send(command);
        return Created(uri: "", result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProjeCommand command)
    {
        UpdatedProjeResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        DeleteProjeCommand command = new() { Id = id };
        DeletedProjeResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdProjeQuery query = new() { Id = id };
        GetByIdProjeResponse? result = await Mediator.Send(query);
        if (result == null) return NotFound("Proje bulunamadı.");
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        GetListProjeQuery query = new();
        List<GetListProjeListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("Assign")]
    public async Task<IActionResult> Assign([FromBody] AssignStajyerProjeCommand command)
    {
        AssignedStajyerProjeResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("StajyerProjeleri")]
    public async Task<IActionResult> GetStajyerProjeleri([FromQuery] int? stajBasvuruId, [FromQuery] int? userId, [FromQuery] int? projeId)
    {
        GetStajyerProjeleriQuery query = new()
        {
            StajBasvuruId = stajBasvuruId,
            UserId = userId,
            ProjeId = projeId
        };
        List<GetStajyerProjeleriListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("StajyerProjeDurum")]
    public async Task<IActionResult> UpdateStajyerProjeDurum([FromBody] UpdateStajyerProjeDurumCommand command)
    {
        bool result = await Mediator.Send(command);
        return Ok(new { success = result });
    }
}