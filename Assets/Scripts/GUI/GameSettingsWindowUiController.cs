using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsWindowUiController : WindowUiController
{
    [SerializeField] private KeybindsSectionUiController keybindsSectionUiController;

    public override void Initialize()
    {
        base.Initialize();

        keybindsSectionUiController.GenerateKeybinds();

        CloseWindow();
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        keybindsSectionUiController.ApplyDataToUi();
    }

    public void Save()
    {
        keybindsSectionUiController.ApplyUiToData();
    }
}
