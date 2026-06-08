using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class MatchMakingController : MonoBehaviour
{
    public static MatchMakingController instance;

    private Coroutine peerConnectionTimeoutCouroutine;

    public event Action onStartMatchMakingSuccess;
    public event Action<string> onStartMatchMakingFailed;
    public event Action onPeerConnecting;
    public event Action onPeerConnected;
    public event Action<string> onPeerConnectionFailed;

    private bool isMatchMaking = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        P2PManager.instance.OnReliableData -= handlePacketReceived;
        P2PManager.instance.OnReady -= handlePeerConnectionReady;
        P2PManager.instance.OnConnected -= handleOnPeerConnected;
    }

    public async void StartMatchMaking()
    {
        if (isMatchMaking) return;

        Debug.Log("[MatchMaking] Starting...");

        isMatchMaking = true;

        WebSocketService.instance.OnConnected += handleWsConnected;

        WebSocketService.instance.OnDisconnected += handleWsConnectFailed;

        // 1. Connect WS
        await WebSocketService.instance.Connect();
    }

    public void CancelMatchMaking()
    {
        if (!isMatchMaking) return;

        isMatchMaking = false;

        Debug.Log("[MatchMaking] Cancelling...");

        // 1. Disconnect WS
        WebSocketService.instance.Disconnect();

        // 2. Clear listener
        WebSocketService.instance.OnMessageReceived -= OnWsMessageReceived;

        // 3. Send cancel match making request
        PlayerMatchMakingService.instance.CancelMatchMaking(
            (response) =>
            {
                Debug.Log("[MatchMaking] Cancelled successfully");
            },
            (code, error) =>
            {
                Debug.LogError($"[MatchMaking] Cancel error {code}: {error}");
            }
        );
    }

    private void handleWsConnectFailed() {
        isMatchMaking = false;

        onStartMatchMakingFailed?.Invoke($"Matchmaking failed: Can't connect to server.");
    }

    private void handleWsConnected() {
        WebSocketService.instance.OnConnected -= handleWsConnected;

        WebSocketService.instance.OnDisconnected -= handleWsConnectFailed;

        // 2. Handle message received
        WebSocketService.instance.OnMessageReceived += OnWsMessageReceived;

        // 3. Send start match making request
        PlayerMatchMakingService.instance.StartMatchMaking(
            (response) =>
            {
                Debug.Log($"[MatchMaking] Success: Match ID {response.id}");
                MatchData.matchId = response.id;
                MatchData.hostPlayer = response.hostPlayer;

                P2PManager.instance.OnReady += handlePeerConnectionReady;

                P2PManager.instance.OnConnected += handleOnPeerConnected;

                P2PManager.instance.OnReliableData += handlePacketReceived;

                if (PlayerData.instance.username == MatchData.hostPlayer)
                {
                    Debug.Log($"[MatchMaking] Host - Waiting for match making...");
                    P2PManager.instance.Init(MatchData.matchId);
                }
                else
                {
                    P2PManager.instance.Init(PlayerData.instance.username + MatchData.matchId);
                }

                onStartMatchMakingSuccess?.Invoke();
            },
            (code, error) =>
            {
                Debug.LogError($"[MatchMaking] Error {code}: {error}");
                onStartMatchMakingFailed?.Invoke($"Matchmaking failed: {error}");
                isMatchMaking = false;
            }
        );
    }

    private void OnWsMessageReceived(WsMessage message)
    {
        Debug.Log($"[MatchMaking] WS Message received: {message.type}");
        if (message.type.CompareTo("MATCH_FOUND") == 0)
        {
            handleHostMatchFound();
            handleMatchFound();
        }

    }

    private void handlePeerConnectionReady()
    {
        P2PManager.instance.OnReady -= handlePeerConnectionReady;

        if (PlayerData.instance.username.CompareTo(MatchData.hostPlayer) == 0) 
            return;
        
        handleClientMatchFound();
        handleMatchFound();
    }

    private void handleMatchFound()
    {
        Debug.Log($"[MatchMaking] Match found.");

        // 1. Disconnect WS
        WebSocketService.instance.Disconnect();

        // 2. Clear listener
        WebSocketService.instance.OnMessageReceived -= OnWsMessageReceived;

        peerConnectionTimeoutCouroutine = StartCoroutine(peerConnectionTimeout());
    }

    private void handleHostMatchFound()
    {
        Debug.Log($"[MatchMaking] Host - Waiting for Client connect to...");
        onPeerConnecting?.Invoke();
    }

    private void handleClientMatchFound()
    {
        Debug.Log($"[MatchMaking] Client - Connecting to Host...");
        onPeerConnecting?.Invoke();
        P2PManager.instance.ConnectTo(MatchData.matchId);
    }

    IEnumerator peerConnectionTimeout()
    {
        yield return new WaitForSecondsRealtime(300f);

        onPeerConnectionFailed?.Invoke("[MatchMaking] Can't connect to Player.");

        Debug.Log("[MatchMaking] Can't connect to Player.");

        peerConnectionTimeoutCouroutine = null;

        P2PManager.instance.OnReady -= handlePeerConnectionReady;

        P2PManager.instance.OnConnected -= handleOnPeerConnected;

        P2PManager.instance.OnReliableData -= handlePacketReceived;

        P2PManager.instance.DisconnectFromPeer();

        isMatchMaking = true;

        StartMatchMaking();
    }

    private void handleOnPeerConnected()
    {
        Debug.Log($"[MatchMaking] Peer connected.");

        StopCoroutine(peerConnectionTimeoutCouroutine);

        P2PManager.instance.OnConnected -= handleOnPeerConnected;

        onPeerConnected?.Invoke();

        peerConnectionTimeoutCouroutine = StartCoroutine(peerConnectionTimeout());

        if (PlayerData.instance.username.CompareTo(MatchData.hostPlayer) != 0)
        {
            ClientInfoPacket packet = new ClientInfoPacket();
            packet.clientUsername = PlayerData.instance.username;
            P2PManager.instance.SendJson(packet);
        }
    }

    private void handlePacketReceived(String data)
    {
        Packet packet = JsonConvert.DeserializeObject<Packet>(data);

        if (packet.type.CompareTo("CLIENT_INFO") == 0 && PlayerData.instance.username.CompareTo(MatchData.hostPlayer) == 0)
        {
            ClientInfoPacket packet2 = JsonConvert.DeserializeObject<ClientInfoPacket>(data);
            MatchData.players.Add(MatchData.hostPlayer);
            MatchData.players.Add(packet2.clientUsername);

            StopCoroutine(peerConnectionTimeoutCouroutine);

            Packet packet1 = new Packet();
            packet1.type = "ACKNOWLEDGE";
            P2PManager.instance.SendJson(packet1);

            loadMatch();
        }
        else if (packet.type.CompareTo("ACKNOWLEDGE") == 0 && PlayerData.instance.username.CompareTo(MatchData.hostPlayer) != 0)
        {
            MatchData.players.Add(MatchData.hostPlayer);
            MatchData.players.Add(PlayerData.instance.username);

            StopCoroutine(peerConnectionTimeoutCouroutine);

            SceneService.instance.LoadSceneDirect("LoadingScene");
        }
        else if (packet.type.CompareTo("LOAD_MATCH") == 0 && PlayerData.instance.username.CompareTo(MatchData.hostPlayer) != 0)
        {
            loadMatch();
        }
    }

    private void loadMatch()
    {
        P2PManager.instance.OnReliableData -= handlePacketReceived;
        SceneService.instance.LoadScene("SampleScene");
    }
}