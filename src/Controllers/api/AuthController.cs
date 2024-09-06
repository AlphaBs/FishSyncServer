using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AlphabetUpdateServer.Controllers.Api;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtAuthService _jwtService;

    public AuthController(
        UserManager<User> userManager,
        IUserStore<User> userStore,
        SignInManager<User> signInManager, 
        JwtAuthService jwtService, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _userStore = userStore;
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
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (result.Succeeded)
        {
            var token = _jwtService.GenerateJwt(request.Username, roles);
            return Ok(new LoginResponseDTO
            {
                Username = request.Username,
                Roles = roles,
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

    [HttpPost("init-role")]
    public async Task<ActionResult> InitRole()
    {
        await _roleManager.CreateAsync(new IdentityRole("user-bucket"));
        await _roleManager.CreateAsync(new IdentityRole("admin-bucket"));
        await _roleManager.CreateAsync(new IdentityRole("admin-storage"));
        await _roleManager.CreateAsync(new IdentityRole("admin-user"));
        return NoContent();
    }

    [HttpPost("init-user")]
    public async Task<ActionResult> InitAdmin()
    {
        var user = new User();
        user.Email = "admin@example.com";
        user.Discord = "";

        await _userStore.SetUserNameAsync(user, "admin", CancellationToken.None);
        var result = await _userManager.CreateAsync(user, "password13");
        await _userManager.AddToRolesAsync(user, [UserRoleNames.UserAdmin, UserRoleNames.BucketAdmin, UserRoleNames.StorageAdmin, UserRoleNames.BucketUser]);

        return Ok(result);
    }
}
