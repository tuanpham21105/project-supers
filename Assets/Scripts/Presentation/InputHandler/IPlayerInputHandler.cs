using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayerInputHandler : MonoBehaviour
{
    public static IPlayerInputHandler instance;
    public PlayerInputController playerInputController;
    void Awake()
    {
        instance = this;
    }
    protected void HandleFlyingInterrupted(String player)
    {
        if (PlayerData.instance.player.CompareTo(player) == 0)
            playerInputController.HandleFlyingInterrupted();
    }
    public abstract void ControlCharacterAction(String player, CharacterActions action, bool state);

    public abstract void ControlCharacterRotation(String player, Vector3 direction);
}
