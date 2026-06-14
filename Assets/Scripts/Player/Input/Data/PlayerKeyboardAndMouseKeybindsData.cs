using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        { "Dash",          new Keybind(KeyCode.LeftShift,    KeyMode.Click,       ActivateAction.Once) },
        { "Toggle Fly",    new Keybind(KeyCode.Space,        KeyMode.DoubleClick, ActivateAction.Toggle) },
        { "Fly Up",        new Keybind(KeyCode.Space,        KeyMode.Click,       ActivateAction.Hold) },
        { "Fly Down",      new Keybind(KeyCode.LeftControl,  KeyMode.Click,       ActivateAction.Hold) },
        { "Normal Attack", new Keybind(KeyCode.Mouse0,       KeyMode.Click,       ActivateAction.Any) },
        { "Strike Attack", new Keybind(KeyCode.E,            KeyMode.Click,       ActivateAction.Any) },
        { "Block",         new Keybind(KeyCode.R,       KeyMode.Click,       ActivateAction.Hold) },
        { "Deflect",       new Keybind(KeyCode.Mouse1,       KeyMode.Click,       ActivateAction.Once) },
        { "Skill 1",       new Keybind(KeyCode.Alpha1,       KeyMode.Click,       ActivateAction.Once) },
        { "Skill 2",       new Keybind(KeyCode.Alpha2,       KeyMode.Click,       ActivateAction.Once) },
        { "Skill 3",       new Keybind(KeyCode.Alpha3,       KeyMode.Click,       ActivateAction.Once) },
    };

    public static String convertKeybindsToString(Dictionary<string, Keybind> keybinds)
    {
        return JsonConvert.SerializeObject(
        keybinds,
        new JsonSerializerSettings
        {
            Converters =
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            }
        });
    }

    public void setKeybindsConfig(String json)
    {
        if (json.Trim() == "" || json == null) return;

        Dictionary<string, Keybind> newKeybinds = JsonConvert.DeserializeObject<Dictionary<string, Keybind>>(
        json,
        new JsonSerializerSettings
        {
            Converters =
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            }
        });

        foreach (var pair in newKeybinds)
        {
            keybinds[pair.Key] = pair.Value;
        }
    }
}

