using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SignUpUiWindow : WindowUiController
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private GameObject emailInputFieldObject;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private GameObject usernameInputFieldObject;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject passwordInputFieldObject;
    [SerializeField] private TextMeshProUGUI ErrorText;
    [SerializeField] private GameObject ErrorTextObject;

    public void SignUpButton()
    {
        string email = emailInputField.text;
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // Email validation
        if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            StartCoroutine(ShowError("Please enter a valid email address."));
            return;
        }

        // Username validation
        if (string.IsNullOrEmpty(username) || username.Length < 6)
        {
            StartCoroutine(ShowError("Username must be at least 6 characters long."));
            return;
        }

        // Password validation
        if (string.IsNullOrEmpty(password) || password.Length < 8 || !Regex.IsMatch(password, @"\d"))
        {
            StartCoroutine(ShowError("Password must be at least 8 characters long and contain at least one number."));
            return;
        }

        PlayerAuthService.instance.SignUp(
            email,
            username,
            password,
            (response) =>
            {
                Debug.Log("[SignUpWindowUiController] Sign up successful.");
                PlayerData.instance.email = "";
                PlayerData.instance.username = "";
                SceneService.instance.ReloadCurrentScene();
            },
            (code, message) =>
            {
                Debug.LogError($"[SignUpWindowUiController] Sign up failed: {message}");
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

        emailInputField.text = "";
        usernameInputField.text = "";
        passwordInputField.text = "";
    }

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        ErrorTextObject.SetActive(false);
    }
}
