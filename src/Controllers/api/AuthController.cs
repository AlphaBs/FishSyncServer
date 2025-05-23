using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services.Users;

namespace AlphabetUpdateServer.Controllers.Api;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly JwtAuthService _jwtService;
    private readonly UserService _userService;

    public AuthController(
        JwtAuthService jwtService,
        UserService userService)
    {
        _jwtService = jwtService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest();
        }

        var user = await _userService.FindUser(request.Username);
        if (user == null)
        {
            return Unauthorized();
        }
        
        if (await _userService.LoginByPassword(user, request.Password))
        {
            var token = _jwtService.GenerateJwt(request.Username, user.Roles);
            return Ok(new LoginResponse
            {
                Username = request.Username,
                Roles = user.Roles,
                Token = token
            });
        }

        return Unauthorized();
    }

    [HttpGet("test")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult Test()
    {
        return Ok(JsonSerializer.Serialize(HttpContext.User.Claims.Select(claim => claim.ToString())));
    }

    [HttpPost("init-user")]
    public async Task<ActionResult> InitAdmin()
    {
        var user = new UserEntity()
        {
            Username = "admin",
            Roles = 
            [
                UserRoleNames.UserAdmin, 
                UserRoleNames.BucketAdmin, 
                UserRoleNames.StorageAdmin, 
                UserRoleNames.BucketUser
            ],
            Email = "admin@example.com",
            Memo = "ADMIN"
        };

        await _userService.AddUser(user, "password13");
        return Ok(user);
    }
}
