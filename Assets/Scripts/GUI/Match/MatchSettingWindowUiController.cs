using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettingWindowUiController : WindowUiController
{
    public static MatchSettingWindowUiController instance;

    public event Action onCloseWindow;

    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public override void Initialize()
    {
        base.Initialize();

        CloseWindow();
    }

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        onCloseWindow?.Invoke();
    }

    public void GameSettings()
    {
        
    }

    public void Surrender()
    {
        MatchManager.instance.StartSurrender();

        CloseWindow();
    }

    public void GoToMainMenu()
    {
        SceneService.instance.LoadScene("StartScene");
    }
}
