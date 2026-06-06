using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EditAccountUiWindowController : WindowUiController
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private GameObject emailInputFieldObject;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private GameObject usernameInputFieldObject;
    [SerializeField] private TextMeshProUGUI ErrorText;
    [SerializeField] private GameObject ErrorTextObject;

    public void Save()
    {
        string email = emailInputField.text;
        string username = usernameInputField.text;

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
        
        PlayerAccountService.instance.UpdatePlayerAccount(
            email,
            username,
            (response) =>
            {
                Debug.Log("[EditAccountUiWindowController] Update account successful.");
                SceneService.instance.ReloadCurrentScene();
            },
            (code, message) =>
            {
                Debug.LogError($"[EditAccountUiWindowController] Update account failed: {message}");
                StartCoroutine(ShowError(message));
            }
        );
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        usernameInputField.text = PlayerData.instance.username;
        emailInputField.text = PlayerData.instance.email;
    }

    IEnumerator ShowError(String errorMsg)
    {
        ErrorText.text = errorMsg;
        ErrorTextObject.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        ErrorTextObject.SetActive(false);
    }

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        ErrorTextObject.SetActive(false);
    }
}
