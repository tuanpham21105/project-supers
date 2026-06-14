using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEditor;
using UnityEngine;

public class KeybindsSectionUiController : MonoBehaviour
{
    [SerializeField] private MouseConfigUiController mouseConfigUiController;

    [SerializeField] private GameObject keybindConfigPrefab;

    private Dictionary<string, KeybindConfigUiController> keybindItemMap;

    [ProButton]
    public void GenerateKeybinds()
    {
        keybindItemMap = new Dictionary<string, KeybindConfigUiController>();

        foreach (var pair in PlayerKeyboardAndMouseKeybindsData.instance.keybinds)
        {
            GameObject go = Instantiate(keybindConfigPrefab, transform);
            go.name = pair.Key;

            KeybindConfigUiController controller = go.GetComponent<KeybindConfigUiController>();
            controller.SetTitle(pair.Key);
            controller.PopulateDropdowns();
            controller.SetSelectedKeyCode(pair.Value.key);
            controller.SetSelectedKeyMode(pair.Value.mode);
            controller.SetSelectedActivateAction(pair.Value.action);

            keybindItemMap[pair.Key] = controller;
        }
    }

    public Keybind GetKeybind(string name)
    {
        if (keybindItemMap != null && keybindItemMap.ContainsKey(name))
        {
            KeybindConfigUiController controller = keybindItemMap[name];
            return new Keybind(
                controller.GetSelectedKeyCode(),
                controller.GetSelectedKeyMode(),
                controller.GetSelectedActivateAction()
            );
        }
        return default;
    }

    public void SetKeybind(string name, Keybind keybind)
    {
        if (keybindItemMap != null && keybindItemMap.ContainsKey(name))
        {
            KeybindConfigUiController controller = keybindItemMap[name];
            controller.SetSelectedKeyCode(keybind.key);
            controller.SetSelectedKeyMode(keybind.mode);
            controller.SetSelectedActivateAction(keybind.action);
        }
    }

    public void ApplyDataToUi()
    {
        foreach (var pair in PlayerKeyboardAndMouseKeybindsData.instance.keybinds)
        {
            SetKeybind(pair.Key, pair.Value);
        }

        mouseConfigUiController.ApplySensitivityToUi(PlayerKeyboardAndMouseKeybindsData.instance.mouseSensitivity);
    }

    public void ApplyUiToData()
    {
        Dictionary<string, Keybind> uiKeybinds = new Dictionary<string, Keybind>();
        foreach (var pair in keybindItemMap)
        {
            uiKeybinds[pair.Key] = GetKeybind(pair.Key);
        }

        string json = PlayerKeyboardAndMouseKeybindsData.convertKeybindsToString(uiKeybinds);
        KeyboardConfigurationRequest request = new KeyboardConfigurationRequest { 
            configuration = json,
            mouseSensitivity = mouseConfigUiController.GetSensitivityFromUi()
        };

        ConfigurationService.instance.PutKeyboardConfiguration(
            request,
            onSuccess: response =>
            {
                PlayerKeyboardAndMouseKeybindsData.instance.setKeybindsConfig(response.configuration);
                PlayerKeyboardAndMouseKeybindsData.instance.mouseSensitivity = response.mouseSensitivity;
                ApplyDataToUi();
            },
            onError: (code, message) =>
            {
                Debug.LogError($"Failed to save keybinds: {code} {message}");
            }
        );
    }
}
