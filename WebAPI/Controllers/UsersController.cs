using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.CreateWithRoles;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.SetRoles;
using Application.Features.Users.Commands.ToggleStatus;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.Commands.UpdateFromAuth;
using Application.Features.Users.Queries.GetById;
using Application.Features.Users.Queries.GetFromAuth;
using Application.Features.Users.Queries.GetList;
using Application.Features.Users.Queries.GetListWithRoles;
using Core.Application.Requests;
using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        GetByIdUserQuery query = new() { Id = id };
        GetByIdUserResponse result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetFromAuth")]
    public async Task<IActionResult> GetFromAuth()
    {
        GetFromAuthUserQuery query = new() { Id = getUserIdFromRequest() };
        GetByIdUserResponse result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserQuery query = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetListWithRoles")]
    public async Task<IActionResult> GetListWithRoles()
    {
        GetListUserWithRolesQuery query = new();
        List<GetListUserWithRolesListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserCommand command)
    {
        CreatedUserResponse result = await Mediator.Send(command);
        return Created(uri: "", result);
    }

    [HttpPost("CreateWithRoles")]
    public async Task<IActionResult> CreateWithRoles([FromBody] CreateUserWithRolesCommand command)
    {
        CreateUserWithRolesResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("SetRoles")]
    public async Task<IActionResult> SetRoles([FromBody] SetUserRolesCommand command)
    {
        SetUserRolesResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("ToggleStatus/{id}")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int id)
    {
        ToggleUserStatusCommand command = new() { UserId = id };
        bool result = await Mediator.Send(command);
        return Ok(new { success = true, status = result });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command)
    {
        UpdatedUserResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("FromAuth")]
    public async Task<IActionResult> UpdateFromAuth([FromBody] UpdateUserFromAuthCommand command)
    {
        command.Id = getUserIdFromRequest();
        UpdatedUserFromAuthResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        DeleteUserCommand command = new() { Id = id };
        DeletedUserResponse result = await Mediator.Send(command);
        return Ok(result);
    }
}