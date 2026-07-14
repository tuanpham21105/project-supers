using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class PlayerMatchInputHandler : PlayerInputHandler
{   
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        playerInputController = PlayerInputController.instance;
        
        CharactersManager.instance.onCharacterFlyingInterrupted += HandleFlyingInterrupted;

        ClientPacketReceiver.instance.onFlyingInterrupted += HandleFlyingInterrupted;

        MatchSettingWindowUiController.instance.onCloseWindow += HandleCloseSetting;
    }

    void OnDestroy()
    {
        instance = null;
        
        CharactersManager.instance.onCharacterFlyingInterrupted -= HandleFlyingInterrupted;

        ClientPacketReceiver.instance.onFlyingInterrupted -= HandleFlyingInterrupted;

        MatchSettingWindowUiController.instance.onCloseWindow -= HandleCloseSetting;
    }

    public void HandleFlyingInterrupted(string player)
    {
        if (PlayerData.instance.username.CompareTo(player) == 0)
            playerInputController.HandleFlyingInterrupted();
    }

    public override void ControlCharacterAction(CharacterActions action, bool state)
    {
        CharactersManager.instance.ControlCharacterAction(PlayerData.instance.username, action, state);
        if (ClientPacketSender.instance != null) 
            ClientPacketSender.instance.sendControlAction(action, state);
    }

    public override void ControlCharacterRotation(Vector3 direction)
    {
        if (CameraController.instance != null) CameraController.instance.Rotate(direction);

        if (MatchManager.instance.IsPlayerHost())
            CharactersManager.instance.ControlCharacterRotation(PlayerData.instance.username, CameraController.instance.GetCameraDirection());
        else
            if (ClientPacketSender.instance != null) 
                ClientPacketSender.instance.sendControlRotation(CameraController.instance.GetCameraDirection());
    }

    public override void OpenSetting()
    {
        MatchSettingWindowUiController.instance.OpenWindow();
    }

    public override void HandleCloseSetting()
    {
        playerInputController.HandleCloseSetting();
    }

    public override void HandleTargetLock(bool state)
    {
        CharactersManager.instance.TargetLock(state);
    }
}
