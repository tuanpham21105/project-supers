using UnityEngine;

public class MatchConnectingUiController : WindowUiController
{
    public override void Initialize()
    {
        MatchManager.instance.onHostGainFocus += CloseWindow;
        MatchManager.instance.onHostLostFocus += OpenWindow;

        MatchConnectionManager.instance.onReconnecting += OpenWindow;
        MatchConnectionManager.instance.onReconnected += CloseWindow;
    }

    void OnDestroy()
    {
        if (MatchManager.instance != null)
        {
            MatchManager.instance.onHostGainFocus -= CloseWindow;
            MatchManager.instance.onHostLostFocus -= OpenWindow;
        }

        if (MatchConnectionManager.instance != null)
        {
            MatchConnectionManager.instance.onReconnecting -= OpenWindow;
            MatchConnectionManager.instance.onReconnected -= CloseWindow;
        }
    }
}