using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class PlayerTrainingAreaInputHandler : PlayerInputHandler
{
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerInputController = PlayerInputController.instance;
        
        TrainingCharacterManager.instance.onCharacterFlyingInterrupted += HandleFlyingInterrupted;

        TrainingAreaSettingWindowUiController.instance.onCloseWindow += HandleCloseSetting;
    }

    void OnDestroy()
    {
        instance = null;
        
        TrainingCharacterManager.instance.onCharacterFlyingInterrupted -= HandleFlyingInterrupted;

        TrainingAreaSettingWindowUiController.instance.onCloseWindow -= HandleCloseSetting;
    }

    public void HandleFlyingInterrupted()
    {
        playerInputController.HandleFlyingInterrupted();
    }

    public override void ControlCharacterAction(CharacterActions action, bool state)
    {
        TrainingCharacterManager.instance.ControlCharacterAction(action, state);
    }

    public override void ControlCharacterRotation(Vector3 direction)
    {
        if (CameraController.instance != null) CameraController.instance.Rotate(direction);

        TrainingCharacterManager.instance.ControlCharacterRotation(CameraController.instance.GetCameraDirection());
    }

    public override void OpenSetting()
    {
        TrainingAreaSettingWindowUiController.instance.OpenWindow();
    }

    public override void HandleCloseSetting()
    {
        playerInputController.HandleCloseSetting();
    }

    public override void HandleTargetLock(bool state)
    {
        DummyCharacterManager.instance.DummyTargetLock(state);
    }
}
