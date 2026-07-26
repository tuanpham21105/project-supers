using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyMode
{
    Click,
    DoubleClick
}

public enum ActivateAction
{
    Once,
    Hold,
    Toggle,
    Any,
}

[System.Serializable]
public struct Keybind
{
    public KeyCode key;
    public KeyMode mode;
    public ActivateAction action;

    public Keybind(KeyCode key, KeyMode mode, ActivateAction action)
    {
        this.key = key;
        this.mode = mode;
        this.action = action;
    }
}

public class PlayerKeyboardAndMouseKeybindsData : MonoBehaviour
{
    public static PlayerKeyboardAndMouseKeybindsData instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Mouse Settings")]
    public float mouseSensitivity = 5f;

    [Header("Input keys")]
    public Dictionary<string, Keybind> keybinds = new Dictionary<string, Keybind>
    {
        { "Move Forward",  new Keybind(KeyCode.W,            KeyMode.Click,       ActivateAction.Any) },
        { "Move Backward", new Keybind(KeyCode.S,            KeyMode.Click,       ActivateAction.Any) },
        { "Strafe Left",   new Keybind(KeyCode.A,            KeyMode.Click,       ActivateAction.Any) },
        { "Strafe Right",  new Keybind(KeyCode.D,            KeyMode.Click,       ActivateAction.Any) },
        { "Jump",          new Keybind(KeyCode.Space,        KeyMode.Click,       ActivateAction.Any) },
        { "Sprint",        new Keybind(KeyCode.LeftShift,    KeyMode.Click,       ActivateAction.Hold) },
        { "Dash",          new Keybind(KeyCode.F,            KeyMode.Click,       ActivateAction.Any) },
        { "Toggle Fly",    new Keybind(KeyCode.Space,        KeyMode.DoubleClick, ActivateAction.Toggle) },
        { "Fly Up",        new Keybind(KeyCode.Space,        KeyMode.Click,       ActivateAction.Hold) },
        { "Fly Down",      new Keybind(KeyCode.LeftControl,  KeyMode.Click,       ActivateAction.Hold) },
        { "Normal Attack", new Keybind(KeyCode.Mouse0,       KeyMode.Click,       ActivateAction.Any) },
        { "Strike Attack", new Keybind(KeyCode.E,            KeyMode.Click,       ActivateAction.Any) },
        { "Block",         new Keybind(KeyCode.Mouse1,       KeyMode.Click,       ActivateAction.Hold) },
        { "Deflect",       new Keybind(KeyCode.Mouse1,       KeyMode.Click,       ActivateAction.Once) },
        { "Target Lock",   new Keybind(KeyCode.Q,            KeyMode.Click,       ActivateAction.Toggle) },
        // { "Skill 1",       new Keybind(KeyCode.Alpha1,       KeyMode.Click,       ActivateAction.Once) },
        // { "Skill 2",       new Keybind(KeyCode.Alpha2,       KeyMode.Click,       ActivateAction.Once) },
        // { "Skill 3",       new Keybind(KeyCode.Alpha3,       KeyMode.Click,       ActivateAction.Once) },
    };

    public static KeyboardConfigValueObject convertKeybindsToValueObject(Dictionary<string, Keybind> keybinds)
    {
        var vo = new KeyboardConfigValueObject();
        vo.keybinds = new List<KeybindValueObject>();

        foreach (var pair in keybinds)
        {
            vo.keybinds.Add(new KeybindValueObject
            {
                actionName = pair.Key,
                keycode = pair.Value.key.ToString(),
                keyMode = pair.Value.mode.ToString(),
                activateAction = pair.Value.action.ToString()
            });
        }

        return vo;
    }

    public void setKeybindsConfig(KeyboardConfigValueObject vo)
    {
        if (vo == null || vo.keybinds == null) return;

        foreach (var voKeybind in vo.keybinds)
        {
            if (string.IsNullOrEmpty(voKeybind.actionName)) continue;

            KeyCode keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), voKeybind.keycode);
            KeyMode mode = (KeyMode)System.Enum.Parse(typeof(KeyMode), voKeybind.keyMode);
            ActivateAction action = (ActivateAction)System.Enum.Parse(typeof(ActivateAction), voKeybind.activateAction);

            keybinds[voKeybind.actionName] = new Keybind(keycode, mode, action);
        }
    }
}

