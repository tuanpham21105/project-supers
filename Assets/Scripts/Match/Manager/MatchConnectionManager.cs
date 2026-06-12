using System;
using System.Collections;
using UnityEngine;

public class MatchConnectionManager : MonoBehaviour
{
    public static MatchConnectionManager instance;

    public event Action onReconnecting;
    public event Action onReconnected;
    public event Action OnDisconnected;

    [SerializeField] private float reconnectDelay = 2f;
    [SerializeField] private float reconnectWait = 5f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void OnDestroy()
    {
        instance = null;

        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnConnected -= handleOnPeerConnected;
            P2PManager.instance.OnDisconnected -= handleOnPeerDisconnected;
        }
    }

    void handleOnPeerDisconnected()
    {
        P2PManager.instance.OnDisconnected -= handleOnPeerDisconnected;
        P2PManager.instance.OnConnected += handleOnPeerConnected;
        Debug.Log("[MatchConnectionManager] Disconnected.");
        OnDisconnected?.Invoke();
        onReconnecting?.Invoke();

        if (MatchManager.instance.IsPlayerHost())
            StartCoroutine(ReconnectHost());
        else
            StartCoroutine(ReconnectClient());
    }

    void handleOnPeerConnected()
    {
        P2PManager.instance.OnDisconnected += handleOnPeerDisconnected;
        P2PManager.instance.OnDisconnected += handleOnPeerDisconnected;
        Debug.Log("[MatchConnectionManager] Reconnected.");
    }

    IEnumerator ReconnectHost()
    {
        P2PManager.instance.Disconnect();
        yield return new WaitForSecondsRealtime(reconnectDelay);

        bool initSuccess = false;
        for (int i = 0; i < 3; i++)
        {
            bool ready = false;
            bool error = false;

            Action onReady = () => ready = true;
            Action<string> onError = (e) => error = true;

            P2PManager.instance.OnReady += onReady;
            P2PManager.instance.OnError += onError;

            P2PManager.instance.Init(MatchData.matchId);

            yield return new WaitUntil(() => ready || error);

            P2PManager.instance.OnReady -= onReady;
            P2PManager.instance.OnError -= onError;

            if (ready)
            {
                initSuccess = true;
                break;
            }

            if (i < 2)
                yield return new WaitForSecondsRealtime(reconnectDelay);
        }

        if (!initSuccess)
        {
            MatchFinishManager.instance.Finish();
            yield break;
        }

        yield return new WaitForSecondsRealtime(reconnectWait * 4f);

        if (!P2PManager.instance.IsConnected)
        {
            MatchFinishManager.instance.Finish();
            yield break;
        }

        onReconnected?.Invoke();
    }

    IEnumerator ReconnectClient()
    {
        P2PManager.instance.Disconnect();
        yield return new WaitForSecondsRealtime(reconnectDelay);

        P2PManager.instance.Init(PlayerData.instance.username + " - " + MatchData.matchId);

        bool ready = false;
        Action onReady = () => ready = true;
        P2PManager.instance.OnReady += onReady;
        yield return new WaitUntil(() => ready);
        P2PManager.instance.OnReady -= onReady;

        bool connected = false;
        for (int i = 0; i < 3; i++)
        {
            Action onConnectedHandler = null;
            onConnectedHandler = () =>
            {
                connected = true;
                P2PManager.instance.OnConnected -= onConnectedHandler;
            };
            P2PManager.instance.OnConnected += onConnectedHandler;

            P2PManager.instance.ConnectTo(MatchData.matchId);

            yield return new WaitForSecondsRealtime(reconnectWait);

            P2PManager.instance.OnConnected -= onConnectedHandler;

            if (connected || P2PManager.instance.IsConnected)
            {
                connected = true;
                break;
            }
        }

        if (!connected)
        {
            MatchFinishManager.instance.Finish();
            yield break;
        }

        onReconnected?.Invoke();
    }
}