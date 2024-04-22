using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AlphabetUpdateServer.Controllers.Api;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtAuthService _jwtService;

    public AuthController(SignInManager<User> signInManager, JwtAuthService jwtService, RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _jwtService = jwtService;
        _roleManager = roleManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDTO request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest();
        }

        var user = await _signInManager.UserManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await _signInManager.UserManager.GetRolesAsync(user);
        var role = string.Join(',', roles);

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (result.Succeeded)
        {
            var token = _jwtService.GenerateJwt(request.Username, role);
            return Ok(new LoginResponseDTO
            {
                Username = request.Username,
                Role = role,
                Token = token
            });
        }
        if (result.RequiresTwoFactor)
        {
            return Unauthorized("required two-factor");
        }
        if (result.IsLockedOut)
        {
            return Unauthorized("locked");
        }

        return Unauthorized();
    }

    [HttpGet("test")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult Test()
    {
        return Ok(JsonSerializer.Serialize(HttpContext.User.Claims.Select(claim => claim.ToString())));
    }

    [HttpPost("init")]
    public async Task<ActionResult> Init()
    {
        await _roleManager.CreateAsync(new IdentityRole("user-bucket"));
        await _roleManager.CreateAsync(new IdentityRole("admin-bucket"));
        await _roleManager.CreateAsync(new IdentityRole("admin-storage"));
        await _roleManager.CreateAsync(new IdentityRole("admin-user"));
        return NoContent();
    }
}
