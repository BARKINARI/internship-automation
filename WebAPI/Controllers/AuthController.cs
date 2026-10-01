using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.RevokeToken;
using Core.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
    {
        RegisterCommand command = new()
        {
            UserForRegisterDto = userForRegisterDto,
            IpAddress = getIpAddress()
        };
        RegisteredResponse result = await Mediator.Send(command);
        setRefreshTokenToCookie(result.RefreshToken);
        return Created(uri: "", result.AccessToken);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
    {
        LoginCommand command = new()
        {
            UserForLoginDto = userForLoginDto,
            IpAddress = getIpAddress()
        };
        LoggedResponse result = await Mediator.Send(command);
        setRefreshTokenToCookie(result.RefreshToken);
        return Ok(new
        {
            token = result.AccessToken.Token,
            expirationDate = result.AccessToken.ExpirationDate,
            firstName = result.FirstName,
            lastName = result.LastName
        });
    }

    [HttpGet("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        string? refreshToken = getRefreshTokenFromCookies();
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest("Refresh token bulunamadı.");

        RefreshTokenCommand command = new()
        {
            RefreshToken = refreshToken,
            IpAddress = getIpAddress()
        };
        RefreshedTokensResponse result = await Mediator.Send(command);
        setRefreshTokenToCookie(result.RefreshToken);
        return Ok(result.AccessToken);
    }

    [HttpPut("RevokeToken")]
    public async Task<IActionResult> RevokeToken([FromBody] string? token)
    {
        string? tokenToRevoke = token ?? getRefreshTokenFromCookies();
        if (string.IsNullOrEmpty(tokenToRevoke))
            return BadRequest("İptal edilecek token bulunamadı.");

        RevokeTokenCommand command = new()
        {
            Token = tokenToRevoke,
            IpAddress = getIpAddress()
        };
        RevokedTokenResponse result = await Mediator.Send(command);
        return Ok(result);
    }

    private string? getRefreshTokenFromCookies() => Request.Cookies["refreshToken"];

    private void setRefreshTokenToCookie(string refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append(key: "refreshToken", value: refreshToken, options: cookieOptions);
    }
}