using System;
using System.Collections;
using UnityEngine;

public class MatchConnectionManager : MonoBehaviour
{
    public static MatchConnectionManager instance;
    public bool IsConnected = true;

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
        if (MatchManager.instance.GetPlayers().Count <= 1) 
            return;

        if (P2PManager.instance.IsConnected)
            P2PManager.instance.OnDisconnected += handleOnPeerDisconnected;
        else    
            handleOnPeerDisconnected();
    }

    void OnDestroy()
    {
        instance = null;

        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnDisconnected -= handleOnPeerDisconnected;
        }
    }

    void handleOnPeerDisconnected()
    {
        P2PManager.instance.OnDisconnected -= handleOnPeerDisconnected;
        Debug.Log("[MatchConnectionManager] Disconnected.");
        OnDisconnected?.Invoke();
        Debug.Log("[MatchConnectionManager] Reconnecting...");
        onReconnecting?.Invoke();

        IsConnected = false;

        if (MatchManager.instance.IsPlayerHost())
            StartCoroutine(ReconnectHost());
        else
            StartCoroutine(ReconnectClient());
    }

    void handleOnPeerConnected()
    {
        P2PManager.instance.OnDisconnected += handleOnPeerDisconnected;

        onReconnected?.Invoke();
        Debug.Log("[MatchConnectionManager] Reconnected.");

        IsConnected = true;
    }

    IEnumerator ReconnectHost()
    {
        P2PManager.instance.DestroyPeer();
        yield return new WaitForSecondsRealtime(reconnectDelay);

        bool initSuccess = false;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"[MatchConnectionManager] Retrying... (attempt {i + 1}/3)");
            bool ready = false;
            bool error = false;

            Action onReady = () => ready = true;
            Action<string> onError = (e) => error = true;

            P2PManager.instance.OnReady += onReady;
            P2PManager.instance.OnError += onError;

            P2PManager.instance.Init(PlayerData.instance.username);

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
            Debug.Log($"[MatchConnectionManager] Can't connect to relay server");

            handleOnPeerReconnectFail();
            yield break;
        }

        yield return new WaitForSecondsRealtime(reconnectWait * 6);

        if (!P2PManager.instance.IsConnected)
        {
            Debug.Log($"[MatchConnectionManager] Cleint not reconnected");

            handleOnPeerReconnectFail();
            yield break;
        }

        handleOnPeerConnected();
    }

    IEnumerator ReconnectClient()
    {
        P2PManager.instance.DestroyPeer();
        yield return new WaitForSecondsRealtime(reconnectDelay);

        P2PManager.instance.Init(PlayerData.instance.username);

        bool ready = false;
        Action onReady = () => ready = true;
        P2PManager.instance.OnReady += onReady;
        yield return new WaitUntil(() => ready);
        P2PManager.instance.OnReady -= onReady;

        bool connected = false;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"[MatchConnectionManager] Retrying... (attempt {i + 1}/3)");
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
            Debug.Log($"[MatchConnectionManager] Can't connect to host");

            handleOnPeerReconnectFail();
            yield break;
        }
        
        handleOnPeerConnected();
    }

    void handleOnPeerReconnectFail()
    {
        Debug.Log($"[MatchConnectionManager] Reconnecting failed");

        Debug.LogWarning("Peer fail");
        MatchFinishManager.instance.Finish("");
    }
}