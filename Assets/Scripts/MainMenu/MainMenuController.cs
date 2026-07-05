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

                MainMenuHeaderUiController.instance.SetHeaderUsername();

                MainMenuSidebarUiController.instance.SetupSidebarUi();

                FetchAndAssignPlayerInventoryAndConfigsData();
            },
            (code, message) =>
            {
                Debug.LogError($"[MainMenuController] Failed to fetch player account: {message}");
                PlayerAuthService.instance.Logout();
                SceneService.instance.ReloadCurrentScene();
            }
        );
    }

    void FetchAndAssignPlayerInventoryAndConfigsData()
    {
        ConfigurationService.instance.GetKeyboardConfiguration(
            (response) =>
            {
                PlayerKeyboardAndMouseKeybindsData.instance.setKeybindsConfig(response.configuration);
                PlayerKeyboardAndMouseKeybindsData.instance.mouseSensitivity = response.mouseSensitivity;
                Debug.Log($"[MainMenuController] Player keyboard configuration loaded");
            },
            (code, message) =>
            {
                Debug.LogError($"[MainMenuController] Failed to fetch player keyboard configuration: {message}");
            }
        );

        PlayerInventoryService.instance.GetPlayerInventory(
            (response) =>
            {
                PlayerData.instance.Points = response.points;
                PlayerData.instance.levels = response.levels;
                PlayerData.instance.exp = response.exp;
                PlayerData.instance.levelsUpExp = response.levelsUpExp;
                Debug.Log($"[MainMenuController] Player inventory loaded");
                
                MainMenuHeaderUiController.instance.SetPoints();
                MainMenuHeaderUiController.instance.SetLevels();
            },
            (code, message) =>
            {
                Debug.LogError($"[MainMenuController] Failed to fetch player inventory: {message}");
            }
        );

        PlayerInventoryService.instance.GetPlayerAccessoriesSet(
            (response) =>
            {
                SetupPlayerAccessoriesData(response);

                SetupPlayerCharacterData(response.character);

                Debug.Log($"[MainMenuController] Player accessories set loaded.");
            },
            (code, message) =>
            {
                Debug.LogError($"[MainMenuController] Failed to fetch player accessories set: {message}");
            }
        );
    }

    void SetupPlayerAccessoriesData(PlayerAccessoriesSetResponse response)
    {
        PlayerData.instance.characterAccessories = new CharacterAccessoriesSet
        {
            hatItem = CharacterAccessory.MapAccessoryFromResponse(response.hatItem),
            maskItem = CharacterAccessory.MapAccessoryFromResponse(response.maskItem),
            neckItem = CharacterAccessory.MapAccessoryFromResponse(response.neckItem),
            chestItem = CharacterAccessory.MapAccessoryFromResponse(response.chestItem),
            backItem = CharacterAccessory.MapAccessoryFromResponse(response.backItem),
            shouldersItem = CharacterAccessory.MapAccessoryFromResponse(response.shouldersItem),
            glovesItem = CharacterAccessory.MapAccessoryFromResponse(response.glovesItem),
            hipItem = CharacterAccessory.MapAccessoryFromResponse(response.hipItem),
            legItem = CharacterAccessory.MapAccessoryFromResponse(response.legItem),
            bootsItem = CharacterAccessory.MapAccessoryFromResponse(response.bootsItem),
        };

        MainMenuCharacterModelController.instance.SetPlayerCharacterAccessoriesFromPlayerData();
    }

    void SetupPlayerCharacterData(PlayerCharacterResponse response)
    {
        if (response != null)
        {
            PlayerData.instance.characterCustomizies.convertFromResponse(response);
        }

        MainMenuCharacterModelController.instance.SetCharacterCustomiziesFromPlayerData();
    }

    [SerializeField] private GameObject openedWindow;

    public void OpenWindow(GameObject window)
    {
        openedWindow.GetComponent<WindowUiController>().CloseWindow();

        openedWindow = window;

        openedWindow.GetComponent<WindowUiController>().OpenWindow();
    }
}
