using UnityEngine;

public class MatchConnectingUiController : WindowUiController
{
    void Start()
    {
        if (window == null)
            window = gameObject;

        MatchManager.instance.onHostGainFocus += CloseWindow;
        MatchManager.instance.onHostLostFocus += OpenWindow;
    }

    void OnDestroy()
    {
        if (MatchManager.instance != null)
        {
            MatchManager.instance.onHostGainFocus -= CloseWindow;
            MatchManager.instance.onHostLostFocus -= OpenWindow;
        }
    }
}