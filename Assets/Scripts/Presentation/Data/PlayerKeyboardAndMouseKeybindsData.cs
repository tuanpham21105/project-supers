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
    Toggle
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
    public static PlayerKeyboardAndMouseKeybindsData playerKeyboardAndMouseKeybindsData;

    private void Awake()
    {
        playerKeyboardAndMouseKeybindsData = this;
    }

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;

    [Header("Movement keys")]
    public Keybind forwardKey = new Keybind(KeyCode.W, KeyMode.Click, ActivateAction.Hold);
    public Keybind backwardKey = new Keybind(KeyCode.S, KeyMode.Click, ActivateAction.Hold);
    public Keybind strafeLeftKey = new Keybind(KeyCode.A, KeyMode.Click, ActivateAction.Hold);
    public Keybind strafeRightKey = new Keybind(KeyCode.D, KeyMode.Click, ActivateAction.Hold);
    public Keybind jumpKey = new Keybind(KeyCode.Space, KeyMode.Click, ActivateAction.Hold);
    public Keybind sprintKey = new Keybind(KeyCode.LeftShift, KeyMode.Click, ActivateAction.Hold);
    public Keybind dashKey = new Keybind(KeyCode.LeftShift, KeyMode.Click, ActivateAction.Hold);
    public Keybind toggleFlyKey = new Keybind(KeyCode.Space, KeyMode.DoubleClick, ActivateAction.Toggle);
    public Keybind flyUpKey = new Keybind(KeyCode.Space, KeyMode.Click, ActivateAction.Hold);
    public Keybind flyDownKey = new Keybind(KeyCode.LeftControl, KeyMode.Click, ActivateAction.Hold);
    public Keybind normalAttackKey = new Keybind(KeyCode.Mouse0, KeyMode.Click, ActivateAction.Hold);
    public Keybind strikeAttackKey = new Keybind(KeyCode.E, KeyMode.Click, ActivateAction.Hold);
    public Keybind blockKey = new Keybind(KeyCode.Mouse1, KeyMode.Click, ActivateAction.Hold);
    public Keybind parryKey = new Keybind(KeyCode.Mouse1, KeyMode.Click, ActivateAction.Once);
}

