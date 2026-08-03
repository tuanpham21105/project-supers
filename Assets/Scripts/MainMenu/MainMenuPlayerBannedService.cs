using System;
using System.Collections;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MainMenuPlayerBannedService : MonoBehaviour
{
    public static MainMenuPlayerBannedService instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        WebSocketService.instance.OnMessageReceived += handleWsMessage;
    }

    void OnDestroy()
    {
        WebSocketService.instance.OnMessageReceived -= handleWsMessage;
    }

    void handleWsMessage(WsMessage message)
    {
        if (message.type.Equals("PLAYER_BANNED"))
        {
            Debug.LogError("You have been banned.");
            PlayerAuthService.instance.Logout();
            SceneService.instance.ReloadCurrentScene();
        }
    }
}