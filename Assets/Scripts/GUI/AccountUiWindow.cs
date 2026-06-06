using TMPro;
using UnityEngine;

public class AccountUiWindow : WindowUiController
{
    [SerializeField] private TMP_InputField usernameTextField;
    [SerializeField] private GameObject usernameTextFieldObject;
    [SerializeField] private TMP_InputField emailTextField;
    [SerializeField] private GameObject emailTextFieldObject;
    [SerializeField] private TMP_InputField createdDateTextField;
    [SerializeField] private GameObject createdDateTextFieldObject;

    public void Logout()
    {
        PlayerAuthService.instance.Logout();
        SceneService.instance.ReloadCurrentScene();
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        usernameTextField.text = PlayerData.instance.username;
        emailTextField.text = PlayerData.instance.email;
        createdDateTextField.text = PlayerData.instance.createdDate;
    }
}
