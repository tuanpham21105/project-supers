public class PlayerAccountUpdateRequest
{
    public string email;
    public string username;

    public PlayerAccountUpdateRequest() {}
    public PlayerAccountUpdateRequest(string email, string username)
    {
        this.email = email;
        this.username = username;
    }
}