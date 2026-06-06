using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EditPasswordWindowUiController : WindowUiController
{
    [SerializeField] private TMP_InputField currentPasswordInputField;
    [SerializeField] private GameObject currentPasswordInputFieldObject;
    [SerializeField] private TMP_InputField newPasswordInputField;
    [SerializeField] private GameObject newPasswordInputFieldObject;
    [SerializeField] private TextMeshProUGUI ErrorText;
    [SerializeField] private GameObject ErrorTextObject;

    public void Save()
    {
        string currentPassword = currentPasswordInputField.text;
        string newPassword = newPasswordInputField.text;

        // Password validation
        if (string.IsNullOrEmpty(currentPassword) || currentPassword.Length < 8 || !Regex.IsMatch(currentPassword, @"\d"))
        {
            StartCoroutine(ShowError("Current password must be at least 8 characters long and contain at least one number."));
            return;
        }
        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8 || !Regex.IsMatch(newPassword, @"\d"))
        {
            StartCoroutine(ShowError("New password must be at least 8 characters long and contain at least one number."));
            return;
        }

        PlayerAccountService.instance.UpdatePlayerPassword(
            new PlayerAccountUpdatePasswordRequest(currentPassword, newPassword),
            (response) =>
            {
                Debug.Log("[EditPasswordWindowUiController] Update password successful.");
                SceneService.instance.ReloadCurrentScene();
            },
            (code, message) =>
            {
                Debug.LogError($"[EditPasswordWindowUiController] Update password failed: {message}");
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

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        ErrorTextObject.SetActive(false);
    }
}
