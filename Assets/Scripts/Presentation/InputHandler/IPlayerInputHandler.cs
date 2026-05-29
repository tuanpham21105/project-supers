using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public abstract class IPlayerInputHandler : MonoBehaviour
{
    public static IPlayerInputHandler instance;
    public PlayerInputController playerInputController;
    void Awake()
    {
        instance = this;
    }
    protected void BaseStart()
    {
        playerInputController = PlayerInputController.instance;
    }
    public void HandleFlyingInterrupted(string player)
    {
        if (PlayerData.instance.player.CompareTo(player) == 0)
            playerInputController.HandleFlyingInterrupted();
    }
    public abstract void ControlCharacterAction(CharacterActions action, bool state);

    public abstract void ControlCharacterRotation(Vector3 direction);
}
