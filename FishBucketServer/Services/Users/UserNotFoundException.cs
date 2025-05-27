namespace AlphabetUpdateServer.Services.Users;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string username) : base("No user: " + username)
    {
        
    }
}