using com.cyborgAssets.inspectorButtonPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuestAccountUiWindowController : WindowUiController
{
    [SerializeField] private TMP_InputField guestUsernameTextField;
    [SerializeField] private GameObject guestUsernameTextFieldObject;
    [SerializeField] private TMP_InputField createdDateextField;
    [SerializeField] private GameObject createdDateTextFieldObject;
    [SerializeField] private TextMeshProUGUI levelsTextField;
    [SerializeField] private TextMeshProUGUI expTextField;
    [SerializeField] private Image expProgressBar;

    public void DeleteGuestAccount()
    {
        PlayerAccountService.instance.DeletePlayerAccount(
            (response) =>
            {
                Debug.Log("[GuestAccountUiWindowController] Delete account successful.");
                PlayerAuthService.instance.Logout();
                PlayerData.instance.Logout();
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
        levelsTextField.text = "Levels: " + PlayerData.instance.levels;
        long levelsUpExp = PlayerData.instance.levelsUpExp;
        expTextField.text = BigNumberStringify.decorate(PlayerData.instance.exp) + " EXP / " + BigNumberStringify.decorate(levelsUpExp) + " EXP";
        expProgressBar.fillAmount = BigNumberStringify.ratio(PlayerData.instance.exp, levelsUpExp);
    }
}
