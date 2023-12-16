using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Controllers;

[Route("users")]
public class UsersController : Controller
{
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(string id)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> AddUser(string id)
    {
        return Ok();
    }

    [HttpPost("{id}")]
    public async Task<ActionResult> UpdateUser(string id)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        return Ok();
    }
}