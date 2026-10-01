using Application.Features.Mesajlar.Commands.Create;
using Application.Features.Mesajlar.Queries.GetBasvuruBilgi;
using Application.Features.Mesajlar.Queries.GetBasvuruByEmail;
using Application.Features.Mesajlar.Queries.GetListByStajBasvuruId;
using Application.Features.Mesajlar.Queries.GetUnreadCounts;
using Application.Features.Mesajlar.Queries.OgrenciGiris;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MesajlarController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] CreateMesajCommand command)
    {
        CreatedMesajResponse result = await Mediator.Send(command);
        return Created(uri: "", result);
    }

    [HttpGet("Konusma/{stajBasvuruId}")]
    public async Task<IActionResult> GetByStajBasvuruId([FromRoute] int stajBasvuruId, [FromQuery] int? projeId = null, [FromQuery] int? okuyanUserId = null, [FromQuery] string? okuyanRol = null)
    {
        GetListByStajBasvuruIdMesajQuery query = new() { StajBasvuruId = stajBasvuruId, ProjeId = projeId, OkuyanUserId = okuyanUserId, OkuyanRol = okuyanRol };
        List<GetListByStajBasvuruIdMesajListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("OgrenciGiris")]
    public async Task<IActionResult> OgrenciGiris([FromBody] OgrenciGirisQuery query)
    {
        OgrenciGirisResponse result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("BasvuruBilgi/{stajBasvuruId}")]
    public async Task<IActionResult> GetBasvuruBilgi([FromRoute] int stajBasvuruId)
    {
        GetBasvuruBilgiMesajQuery query = new() { StajBasvuruId = stajBasvuruId };
        OgrenciBasvuruBilgiDto? result = await Mediator.Send(query);
        if (result == null) return NotFound("Staj başvurusu bulunamadı.");
        return Ok(result);
    }

    [HttpGet("GetBasvuruByEmail/{email}")]
    public async Task<IActionResult> GetBasvuruByEmail([FromRoute] string email)
    {
        GetBasvuruByEmailMesajQuery query = new() { Email = Uri.UnescapeDataString(email) };
        OgrenciBasvuruBilgiDto? result = await Mediator.Send(query);
        if (result == null) return NotFound("Staj başvurusu bulunamadı.");
        return Ok(result);
    }

    [HttpGet("UnreadCounts")]
    public async Task<IActionResult> GetUnreadCounts([FromQuery] int aliciId)
    {
        GetUnreadCountsMesajQuery query = new() { AliciId = aliciId };
        Dictionary<int, int> result = await Mediator.Send(query);
        return Ok(result);
    }
}
