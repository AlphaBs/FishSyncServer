using Microsoft.AspNetCore.Identity;

namespace AlphabetUpdateServer.Areas.Identity.Data;

public class User : IdentityUser
{
    public string? Discord { get; set; }
}