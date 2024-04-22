namespace AlphabetUpdateServer.DTOs;

public class LoginResponseDTO
{
    public required string Username { get; init; }
    public required string Role { get; init; }
    public required string Token { get; init; }
}
