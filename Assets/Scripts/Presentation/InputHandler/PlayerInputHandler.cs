using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler instance;
    public PlayerInputController playerInputController;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerInputController = PlayerInputController.instance;

        CharactersManager.instance.onCharacterFlyingInterrupted += HandleFlyingInterrupted;

        ClientPacketReceiver.instance.onFlyingInterrupted += HandleFlyingInterrupted;
    }

    void OnDestroy()
    {
        CharactersManager.instance.onCharacterFlyingInterrupted -= HandleFlyingInterrupted;

        ClientPacketReceiver.instance.onFlyingInterrupted -= HandleFlyingInterrupted;
    }

    public void HandleFlyingInterrupted(string player)
    {
        if (PlayerData.instance.player.CompareTo(player) == 0)
            playerInputController.HandleFlyingInterrupted();
    }

    public void ControlCharacterAction(CharacterActions action, bool state)
    {
        if (MatchManager.instance.IsPlayerHost())
            CharactersManager.instance.ControlCharacterAction(PlayerData.instance.player, action, state);
        else
            ClientPacketSender.instance.sendControlAction(action, state);
    }

    public void ControlCharacterRotation(Vector3 direction)
    {
        if (MatchManager.instance.IsPlayerHost())
            CharactersManager.instance.ControlCharacterRotation(PlayerData.instance.player, direction);
        else
            ClientPacketSender.instance.sendControlRotation(direction);
    }
}
