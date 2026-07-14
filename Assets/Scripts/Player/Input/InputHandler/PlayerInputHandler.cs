using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public abstract class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler instance;
    public PlayerInputController playerInputController;

    public abstract void ControlCharacterAction(CharacterActions action, bool state);

    public abstract void ControlCharacterRotation(Vector3 direction);

    public abstract void OpenSetting();

    public abstract void HandleCloseSetting();

    public abstract void HandleTargetLock(bool state);
}
