namespace Web.Services;

public class UserState
{
    public string Username { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Username);

    public void SetUser(string username)
    {
        Username = username;
    }

    public void ClearUser()
    {
        Username = null;
    }
}