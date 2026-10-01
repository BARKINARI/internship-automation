using Application.Features.StajyerProjeler.Commands.AssignStajyer;
using Application.Features.StajyerProjeler.Commands.DeleteStajyerProje;
using Application.Features.StajyerProjeler.Commands.UpdateStajyerProje;
using Application.Features.StajyerProjeler.Queries.GetById;
using Application.Features.StajyerProjeler.Queries.GetList;
using Application.Features.StajyerProjeler.Queries.GetStajyerProjeleri;
using Core.Application.Requests;
using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StajyerProjelerController : BaseController
{
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdStajyerProjeQuery query = new() { Id = id };
        GetByIdStajyerProjeResponse result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListStajyerProjeQuery query = new() { PageRequest = pageRequest };
        GetListResponse<GetListStajyerProjeListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("Durum")]
    public async Task<IActionResult> UpdateDurum([FromBody] UpdateStajyerProjeDurumCommand command)
    {
        bool result = await Mediator.Send(command);
        return Ok(new { success = result });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        DeleteStajyerProjeCommand command = new() { Id = id };
        DeletedStajyerProjeResponse result = await Mediator.Send(command);
        return Ok(result);
    }
}