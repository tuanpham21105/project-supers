using TMPro;
using UnityEngine;

public class GuestAccountUiWindowController : WindowUiController
{
    [SerializeField] private TMP_InputField guestUsernameTextField;
    [SerializeField] private GameObject guestUsernameTextFieldObject;
    [SerializeField] private TMP_InputField createdDateextField;
    [SerializeField] private GameObject createdDateTextFieldObject;

    public void DeleteGuestAccount()
    {
        PlayerAccountService.instance.DeletePlayerAccount(
            (response) =>
            {
                Debug.Log("[GuestAccountUiWindowController] Delete account successful.");
                PlayerAuthService.instance.Logout();
                SceneService.instance.ReloadCurrentScene();
            },
            (code, message) =>
            {
                Debug.LogError($"[GuestAccountUiWindowController] Delete account failed: {message}");
            }
        );
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        guestUsernameTextField.text = PlayerData.instance.username;
        createdDateextField.text = PlayerData.instance.createdDate;
    }
}
