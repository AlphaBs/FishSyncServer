namespace AlphabetUpdateServer.DTOs;

public class LoginResponseDTO
{
    public required string Username { get; init; }
    public required IEnumerable<string> Roles { get; init; }
    public required string Token { get; init; }
}
