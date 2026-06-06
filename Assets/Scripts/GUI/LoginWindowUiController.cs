using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoginWindowUiController : WindowUiController
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private GameObject usernameInputFieldObject;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject passwordInputFieldObject;
    [SerializeField] private TextMeshProUGUI ErrorText;
    [SerializeField] private GameObject ErrorTextObject;
    
    public void Login()
    {
        string email = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowError("Email and password are required."));
            return;
        }

        PlayerAuthService.instance.Login(
            email,
            password,
            (response) =>
            {
                Debug.Log("[LoginWindowUiController] Login successful.");
                SceneService.instance.ReloadCurrentScene();
            },
            (code, message) =>
            {
                Debug.LogError($"[LoginWindowUiController] Login failed: {message}");
                StartCoroutine(ShowError(message));
            }
        );
    }

    IEnumerator ShowError(String errorMsg)
    {
        ErrorText.text = errorMsg;
        ErrorTextObject.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        ErrorTextObject.SetActive(false);
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        usernameInputField.text = "";
        passwordInputField.text = "";
    }

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        ErrorTextObject.SetActive(false);
    }
}
