using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;


public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupPlayerAccount();
    }

    [ProButton]
    public void ClearPlayerPrefs() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void SetupPlayerAccount()
    {
        if (PlayerAuthService.instance.IsLoggedIn())
        {
            FetchAndAssignPlayerData();
        }
        else
        {
            PlayerAuthService.instance.CreateGuestAccount(
                (response) =>
                {
                    FetchAndAssignPlayerData();
                },
                (code, message) =>
                {
                    Debug.LogError($"[MainMenuController] Failed to create guest account: {message}");
                }
            );
        }
    }

    private void FetchAndAssignPlayerData()
    {
        PlayerAccountService.instance.GetPlayerAccount(
            (response) =>
            {
                PlayerData.instance.email = response.email;
                PlayerData.instance.username = response.username;
                PlayerData.instance.createdDate = response.createdDate;
                PlayerData.instance.isGuest = response.isGuest;
                Debug.Log($"[MainMenuController] Player data loaded: {response.username}");

                MainMenuSidebarUiController.instance.SetupSidebarUi();
                MainMenuHeaderUiController.instance.SetupHeaderUi();
            },
            (code, message) =>
            {
                Debug.LogError($"[MainMenuController] Failed to fetch player account: {message}");
                PlayerAuthService.instance.Logout();
                SceneService.instance.ReloadCurrentScene();
            }
        );
    }

    [SerializeField] private GameObject openedWindow;

    public void OpenWindow(GameObject window)
    {
        openedWindow.GetComponent<WindowUiController>().CloseWindow();

        openedWindow = window;

        openedWindow.GetComponent<WindowUiController>().OpenWindow();
    }
}
