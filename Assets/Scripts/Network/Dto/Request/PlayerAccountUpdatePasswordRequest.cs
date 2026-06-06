public class PlayerAccountUpdatePasswordRequest
{
    public string oldPassword;
    public string newPassword;

    public PlayerAccountUpdatePasswordRequest() {}

    public PlayerAccountUpdatePasswordRequest(string oldPassword, string newPassword)
    {
        this.oldPassword = oldPassword;
        this.newPassword = newPassword;
    }
}