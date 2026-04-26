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
    public String name;
    public KeyCode key;
    public KeyMode mode;
    public ActivateAction action;

    public Keybind(String name, KeyCode key, KeyMode mode, ActivateAction action)
    {
        this.name = name;
        this.key = key;
        this.mode = mode;
        this.action = action;
    }
}

public class PlayerKeyboardAndMouseKeybindsData : MonoBehaviour
{
    public static PlayerKeyboardAndMouseKeybindsData playerKeyboardAndMouseKeybindsData;

    private void Awake()
    {
        playerKeyboardAndMouseKeybindsData = this;
    }

    [Header("Mouse Settings")]
    public float mouseSensitivity = 5f;

    [Header("Movement keys")]
    public Keybind forwardKey      = new Keybind("Move Forward",  KeyCode.W,            KeyMode.Click,       ActivateAction.Any);
    public Keybind backwardKey     = new Keybind("Move Backward", KeyCode.S,            KeyMode.Click,       ActivateAction.Any);
    public Keybind strafeLeftKey   = new Keybind("Strafe Left",   KeyCode.A,            KeyMode.Click,       ActivateAction.Any);
    public Keybind strafeRightKey  = new Keybind("Strafe Right",  KeyCode.D,            KeyMode.Click,       ActivateAction.Any);
    public Keybind jumpKey         = new Keybind("Jump",          KeyCode.Space,        KeyMode.Click,       ActivateAction.Hold);
    public Keybind sprintKey       = new Keybind("Sprint",        KeyCode.LeftShift,    KeyMode.Click,       ActivateAction.Hold);
    public Keybind dashKey         = new Keybind("Dash",          KeyCode.LeftShift,    KeyMode.Click,       ActivateAction.Once);
    public Keybind toggleFlyKey    = new Keybind("Toggle Fly",    KeyCode.Space,        KeyMode.DoubleClick, ActivateAction.Toggle);
    public Keybind flyUpKey        = new Keybind("Fly Up",        KeyCode.Space,        KeyMode.Click,       ActivateAction.Hold);
    public Keybind flyDownKey      = new Keybind("Fly Down",      KeyCode.LeftControl,  KeyMode.Click,       ActivateAction.Hold);
    public Keybind normalAttackKey = new Keybind("Normal Attack", KeyCode.Mouse0,       KeyMode.Click,       ActivateAction.Any);
    public Keybind strikeAttackKey = new Keybind("Strike Attack", KeyCode.E,            KeyMode.Click,       ActivateAction.Any);
    public Keybind blockKey        = new Keybind("Block",         KeyCode.Mouse1,       KeyMode.Click,       ActivateAction.Hold);
    public Keybind deflectKey      = new Keybind("Deflect",       KeyCode.Mouse1,       KeyMode.Click,       ActivateAction.Once);
}

