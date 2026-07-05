using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountUiWindow : WindowUiController
{
    [SerializeField] private TMP_InputField usernameTextField;
    [SerializeField] private GameObject usernameTextFieldObject;
    [SerializeField] private TMP_InputField emailTextField;
    [SerializeField] private GameObject emailTextFieldObject;
    [SerializeField] private TMP_InputField createdDateTextField;
    [SerializeField] private GameObject createdDateTextFieldObject;
    [SerializeField] private TextMeshProUGUI levelsTextField;
    [SerializeField] private TextMeshProUGUI expTextField;
    [SerializeField] private Image expProgressBar;

    public void Logout()
    {
        Debug.Log("[AccountWindowController] Logout.");
        PlayerAuthService.instance.Logout();
        PlayerData.instance.Logout();
        SceneService.instance.ReloadCurrentScene();
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        usernameTextField.text = PlayerData.instance.username;
        emailTextField.text = PlayerData.instance.email;
        createdDateTextField.text = PlayerData.instance.createdDate;
        levelsTextField.text = "Levels: " + PlayerData.instance.levels;
        long levelsUpExp = PlayerData.instance.levelsUpExp;
        expTextField.text = PlayerData.instance.exp + " EXP / " + levelsUpExp + " EXP";
        expProgressBar.fillAmount = PlayerData.instance.exp / levelsUpExp;
    }
}
