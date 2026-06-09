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
        if (PlayerData.instance.username.CompareTo(player) == 0)
            playerInputController.HandleFlyingInterrupted();
    }

    public void ControlCharacterAction(CharacterActions action, bool state)
    {
        CharactersManager.instance.ControlCharacterAction(PlayerData.instance.username, action, state);
        if (ClientPacketSender.instance != null) 
            ClientPacketSender.instance.sendControlAction(action, state);
    }

    public void ControlCharacterRotation(Vector3 direction)
    {
        if (MatchManager.instance.IsPlayerHost())
            CharactersManager.instance.ControlCharacterRotation(PlayerData.instance.username, direction);
        else
            if (ClientPacketSender.instance != null) 
                ClientPacketSender.instance.sendControlRotation(direction);
    }
}
