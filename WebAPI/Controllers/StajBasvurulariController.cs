using Application.Features.StajBasvurulari.Commands.ApproveAndSendPassword;
using Application.Features.StajBasvurulari.Commands.Create;
using Application.Features.StajBasvurulari.Commands.Delete;
using Application.Features.StajBasvurulari.Commands.Update;
using Application.Features.StajBasvurulari.Queries.GetById;
using Application.Features.StajBasvurulari.Queries.GetList;
using Application.Features.StajBasvurulari.Queries.GetListByDynamic;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistence.Dynamic;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StajBasvurulariController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateStajBasvuruCommand command)
    {
        CreatedStajBasvuruResponse result = await Mediator.Send(command);
        return Created(uri: "", result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStajBasvuruCommand command)
    {
        UpdatedStajBasvuruResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        DeleteStajBasvuruCommand command = new() { Id = id };
        DeletedStajBasvuruResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdStajBasvuruQuery query = new() { Id = id };
        GetByIdStajBasvuruResponse result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest, [FromQuery] bool? bypassCache = null)
    {
        GetListStajBasvuruQuery query = new() { PageRequest = pageRequest };
        if (bypassCache.HasValue) query.BypassCache = bypassCache.Value;
        GetListResponse<GetListStajBasvuruListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetList/ByDynamic")]
    public async Task<IActionResult> GetListByDynamic([FromQuery] PageRequest pageRequest, [FromBody] DynamicQuery dynamicQuery)
    {
        GetListByDynamicStajBasvuruQuery query = new() { PageRequest = pageRequest, DynamicQuery = dynamicQuery };
        GetListResponse<GetListStajBasvuruListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("ApproveAndSendPassword")]
    public async Task<IActionResult> ApproveAndSendPassword([FromBody] ApproveAndSendPasswordCommand command)
    {
        ApproveAndSendPasswordResponse result = await Mediator.Send(command);
        return Ok(result);
    }
}