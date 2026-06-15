using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using TMPro;
using UnityEngine;

public class KeybindConfigUiController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTextField;
    [SerializeField] private TMP_Dropdown keycodeDropdownField;
    [SerializeField] private TMP_Dropdown keyModeDropdownField;
    [SerializeField] private TMP_Dropdown activateActionDropdownField;

    [ProButton]
    private void PopulateKeycodeDropdown()
    {
        keycodeDropdownField.ClearOptions();

        List<string> options = new List<string>();

        for (char c = 'A'; c <= 'Z'; c++)
            options.Add(c.ToString());

        for (char c = '0'; c <= '9'; c++)
            options.Add(c.ToString());

        options.Add("Mouse0");
        options.Add("Mouse1");
        options.Add("Mouse2");
        options.Add("Space");
        options.Add("Tab");
        options.Add("CapsLock");
        options.Add("Shift");
        options.Add("Ctrl");
        options.Add("Alt");
        options.Add("UpArrow");
        options.Add("DownArrow");
        options.Add("LeftArrow");
        options.Add("RightArrow");

        keycodeDropdownField.AddOptions(options);
    }

    [ProButton]
    private void PopulateKeyModeDropdown()
    {
        keyModeDropdownField.ClearOptions();
        keyModeDropdownField.AddOptions(new List<string>
        {
            "Click",
            "DoubleClick"
        });
    }

    [ProButton]
    private void PopulateActivateActionDropdown()
    {
        activateActionDropdownField.ClearOptions();
        activateActionDropdownField.AddOptions(new List<string>
        {
            "Once",
            "Hold",
            "Toggle",
            "Any"
        });
    }

    public void PopulateDropdowns()
    {
        PopulateKeycodeDropdown();
        PopulateKeyModeDropdown();
        PopulateActivateActionDropdown();
    }

    private KeyCode StringToKeyCode(string str)
    {
        switch (str)
        {
            case "0": case "1": case "2": case "3": case "4":
            case "5": case "6": case "7": case "8": case "9":
                return (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + str);
            case "Shift": return KeyCode.LeftShift;
            case "Ctrl":  return KeyCode.LeftControl;
            case "Alt":   return KeyCode.LeftAlt;
            default:      return (KeyCode)System.Enum.Parse(typeof(KeyCode), str);
        }
    }

    private KeyMode StringToKeyMode(string str)
    {
        switch (str)
        {
            case "Click":       return KeyMode.Click;
            case "DoubleClick": return KeyMode.DoubleClick;
            default:            return KeyMode.Click;
        }
    }

    private ActivateAction StringToActivateAction(string str)
    {
        switch (str)
        {
            case "Once":   return ActivateAction.Once;
            case "Hold":   return ActivateAction.Hold;
            case "Toggle": return ActivateAction.Toggle;
            case "Any":    return ActivateAction.Any;
            default:       return ActivateAction.Once;
        }
    }

    private string KeyCodeToString(KeyCode key)
    {
        if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
            return ((int)key - (int)KeyCode.Alpha0).ToString();

        switch (key)
        {
            case KeyCode.LeftShift:
            case KeyCode.RightShift:   return "Shift";
            case KeyCode.LeftControl:
            case KeyCode.RightControl: return "Ctrl";
            case KeyCode.LeftAlt:
            case KeyCode.RightAlt:     return "Alt";
            default:                   return key.ToString();
        }
    }

    private string KeyModeToString(KeyMode mode)
    {
        switch (mode)
        {
            case KeyMode.Click:       return "Click";
            case KeyMode.DoubleClick: return "DoubleClick";
            default:                  return "Click";
        }
    }

    private string ActivateActionToString(ActivateAction action)
    {
        switch (action)
        {
            case ActivateAction.Once:   return "Once";
            case ActivateAction.Hold:   return "Hold";
            case ActivateAction.Toggle: return "Toggle";
            case ActivateAction.Any:    return "Any";
            default:                    return "Once";
        }
    }

    public KeyCode GetSelectedKeyCode()
    {
        string option = keycodeDropdownField.options[keycodeDropdownField.value].text;
        return StringToKeyCode(option);
    }

    public void SetSelectedKeyCode(KeyCode key)
    {
        string option = KeyCodeToString(key);
        for (int i = 0; i < keycodeDropdownField.options.Count; i++)
        {
            if (keycodeDropdownField.options[i].text == option)
            {
                keycodeDropdownField.value = i;
                return;
            }
        }
    }

    public KeyMode GetSelectedKeyMode()
    {
        string option = keyModeDropdownField.options[keyModeDropdownField.value].text;
        return StringToKeyMode(option);
    }

    public void SetSelectedKeyMode(KeyMode mode)
    {
        string option = KeyModeToString(mode);
        for (int i = 0; i < keyModeDropdownField.options.Count; i++)
        {
            if (keyModeDropdownField.options[i].text == option)
            {
                keyModeDropdownField.value = i;
                return;
            }
        }
    }

    public ActivateAction GetSelectedActivateAction()
    {
        string option = activateActionDropdownField.options[activateActionDropdownField.value].text;
        return StringToActivateAction(option);
    }

    public void SetSelectedActivateAction(ActivateAction action)
    {
        string option = ActivateActionToString(action);
        for (int i = 0; i < activateActionDropdownField.options.Count; i++)
        {
            if (activateActionDropdownField.options[i].text == option)
            {
                activateActionDropdownField.value = i;
                return;
            }
        }
    }

    public void SetTitle(string title)
    {
        titleTextField.text = title;
    }

    public string GetTitle()
    {
        return titleTextField.text;
    }
}
