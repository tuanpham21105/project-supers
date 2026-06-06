using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSidebarUiController : MonoBehaviour
{
    public static MainMenuSidebarUiController instance;

    [SerializeField] private GameObject loginButton;

    [SerializeField] private GameObject RegisteredAccountWindow;
    [SerializeField] private GameObject GuestAccountWindow;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    public void SetupSidebarUi()
    {
        loginButton.SetActive(PlayerData.instance.isGuest);
    }

    public void GoToAccount()
    {
        if (PlayerData.instance.isGuest)
            MainMenuController.instance.OpenWindow(GuestAccountWindow);
        else 
            MainMenuController.instance.OpenWindow(RegisteredAccountWindow);
    }
}
