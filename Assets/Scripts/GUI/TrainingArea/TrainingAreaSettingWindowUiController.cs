using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAreaSettingWindowUiController : WindowUiController
{
    public static TrainingAreaSettingWindowUiController instance;

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

    public void GoToMainMenu()
    {
        SceneService.instance.LoadScene("StartScene");
    }

    
}
