using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.cyborgAssets.inspectorButtonPro;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WsMessageType
{
    none,
    MATCH_READY,
    OFFER,
    ANSWER,
    ICE_CANDIDATE
}

public class ReadyMatchDto
{
    public string id;
    public string hostPlayer;
    public string clientPlayer;
}

public class MatchMakingManager : MonoBehaviour
{
    private MatchMakingService matchMakingService;
    private WebSocketService webSocketService;

    void Start()
    {
        matchMakingService = GetComponent<MatchMakingService>();
        webSocketService = GetComponent<WebSocketService>();
    }

    /// <summary>
    /// Starts matchmaking by connecting to WebSocket and requesting a match.
    /// Listens for MATCH_READY message to initialize P2P connection.
    /// </summary>
    [ProButton]
    public void StartMatchMaking()
    {
        if (matchMakingService == null || webSocketService == null)
        {
            Debug.LogError("[MatchMakingManager] MatchMakingService or WebSocketService dependency is missing.");
            return;
        }

        Debug.Log("[MatchMakingManager] Starting matchmaking...");
        ResetMatchData();

        // Connect to WebSocket
        webSocketService.Connect();
        Debug.Log("[MatchMakingManager] WebSocket connected");

        // Subscribe to message events
        webSocketService.OnMessageReceived += HandleMessageReceived;

        // Request a match
        matchMakingService.FindMatch(
            onSuccess: (matchResponse) =>
            {
                Debug.Log($"[MatchMakingManager] Match found: Id={matchResponse.Id}, Host={matchResponse.HostPlayer}");
            },
            onError: (error) =>
            {
                Debug.LogError($"[MatchMakingManager] FindMatch failed: {error}");
            }
        );
    }

    public void CancelMatchMakingExternal() {
        StartCoroutine(CancelMatchMaking());
    }

    /// <summary>
    /// Cancels the matchmaking process and cleans up all connections.
    /// </summary>
    [ProButton]
    public IEnumerator CancelMatchMaking()
    {
        if (matchMakingService == null || webSocketService == null)
        {
            Debug.LogError("[MatchMakingManager] MatchMakingService or WebSocketService dependency is missing.");
            yield break;
        }

        Debug.Log("[MatchMakingManager] Canceling matchmaking...");

        // Unsubscribe from events
        webSocketService.OnMessageReceived -= HandleMessageReceived;

        // Disconnect WebSocket
        webSocketService.Disconnect();
        Debug.Log("[MatchMakingManager] WebSocket disconnected");

        // Disconnect P2P
        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnConnected -= HandleP2PConnected;
            P2PManager.instance.OnReady -= HandleP2PReady;
            P2PManager.instance.OnReliableData -= handleLoadMatch;
            P2PManager.instance.Disconnect();
            Debug.Log("[MatchMakingManager] P2P disconnected");
        }

        // Call cancelMatch API and wait for completion
        bool cancelCompleted = false;
        matchMakingService.CancelMatchMaking(
            onSuccess: (messageResponse) =>
            {
                Debug.Log($"[MatchMakingManager] CancelMatch succeeded: {messageResponse.Message}");
                cancelCompleted = true;
            },
            onError: (error) =>
            {
                Debug.LogError($"[MatchMakingManager] CancelMatch failed: {error}");
                cancelCompleted = true;
            }
        );

        // Wait for API call to complete
        yield return new WaitUntil(() => cancelCompleted);
        Debug.Log("[MatchMakingManager] Matchmaking cancelled successfully");
    }

    private void HandleMessageReceived(Message message)
    {
        if (message == null)
        {
            Debug.LogWarning("[MatchMakingManager] Received null message");
            return;
        }

        Debug.Log($"[MatchMakingManager] Message received - Type: {message.type}, MatchId: {message.matchId}, Value: {message.value}");

        if (message.type.CompareTo(WsMessageType.MATCH_READY.ToString()) == 0)
        {
            HandleMatchReadyMessage(message);
        }
    }

    private void HandleMatchReadyMessage(Message message)
    {
        // if (message.MatchId.CompareTo(MatchData.matchId) != 0)
        // {
        //     Debug.LogWarning($"[MatchMakingManager] Message matchId mismatch: {message.MatchId} vs {MatchData.matchId}");
        //     return;
        // }

        try
        {
            ReadyMatchDto readyMatchDto = message.GetValue<ReadyMatchDto>();
            Debug.Log($"[MatchMakingManager] Match ready - Id: {readyMatchDto.id}, Host: {readyMatchDto.hostPlayer}, Client: {readyMatchDto.clientPlayer}");

            // Assign match data
            MatchData.matchId = readyMatchDto.id;
            MatchData.hostPlayer = readyMatchDto.hostPlayer;
            MatchData.players.Clear();
            MatchData.players.Add(readyMatchDto.hostPlayer);
            MatchData.players.Add(readyMatchDto.clientPlayer);

            // Initialize P2P connection
            if (P2PManager.instance == null)
            {
                Debug.LogError("[MatchMakingManager] P2PManager is not available");
                return;
            }

            // Subscribe to P2P connection event
            P2PManager.instance.OnConnected += HandleP2PConnected;
            P2PManager.instance.OnReady += HandleP2PReady;

            if (PlayerData.instance.player.CompareTo(readyMatchDto.hostPlayer) == 0)
            {
                Debug.Log("[MatchMakingManager] Initializing as HOST");
                P2PManager.instance.Init(readyMatchDto.id);
            }
            else
            {
                Debug.Log("[MatchMakingManager] Initializing as CLIENT");
                P2PManager.instance.Init("Client-" + readyMatchDto.id);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MatchMakingManager] Failed to parse MATCH_READY message: {ex.Message}");
        }
    }


    [ProButton]
    private void HandleP2PConnected()
    {
        Debug.Log("[MatchMakingManager] P2P connected, starting cleanup and scene transition...");
        StartCoroutine(CleanupAndLoadScene());
    }

    private void HandleP2PReady()
    {
        Debug.Log("[MatchMakingManager] P2P ready");

        // Unsubscribe from OnReady event
        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnReady -= HandleP2PReady;
        }

        // Subscribe to LOAD_MATCH packet listener for both host and client
        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnReliableData += handleLoadMatch;
            Debug.Log("[MatchMakingManager] Subscribed to LOAD_MATCH listener");
        }

        // If client, connect to host
        if (PlayerData.instance.player.CompareTo(MatchData.hostPlayer) != 0)
        {
            Debug.Log("[MatchMakingManager] Connecting to match as CLIENT");
            P2PManager.instance.ConnectTo(MatchData.matchId);
        }
        else
        {
            Debug.Log("[MatchMakingManager] Waiting for client connection as HOST");
        }
    }

    private IEnumerator CleanupAndLoadScene()
    {
        // Unsubscribe from all events
        if (webSocketService != null)
        {
            webSocketService.OnMessageReceived -= HandleMessageReceived;
            Debug.Log("[MatchMakingManager] Unsubscribed from WebSocket events");
        }

        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnConnected -= HandleP2PConnected;
            Debug.Log("[MatchMakingManager] Unsubscribed from P2P OnConnected event");
        }

        // Disconnect WebSocket
        if (webSocketService != null)
        {
            webSocketService.Disconnect();
            Debug.Log("[MatchMakingManager] WebSocket disconnected");
        }

        // Only host loads scene first
        if (PlayerData.instance.player.CompareTo(MatchData.hostPlayer) == 0)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("[MatchMakingManager] Host loading SampleScene...");
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("[MatchMakingManager] Client waiting for LOAD_MATCH packet from host...");
        }
    }

    private void handleLoadMatch(string data)
    {
        Packet packet = JsonConvert.DeserializeObject<Packet>(data);

        if (packet.type.CompareTo("LOAD_MATCH") == 0)
        {
            Debug.Log("[MatchMakingManager] Received LOAD_MATCH packet, loading scene...");
            LoadScene();
        }
    }

    public void LoadScene()
    {
        Debug.Log("[MatchMakingManager] Loading SampleScene...");
        SceneManager.LoadScene("SampleScene");
    }

    private void ResetMatchData()
    {
        MatchData.matchId = "";
        MatchData.hostPlayer = "";
        MatchData.players.Clear();
        Debug.Log("[MatchMakingManager] MatchData reset");
    }

    private void OnDestroy()
    {
        // Clean up subscription to prevent memory leaks
        if (webSocketService != null)
        {
            webSocketService.OnMessageReceived -= HandleMessageReceived;
        }

        if (P2PManager.instance != null)
        {
            P2PManager.instance.OnConnected -= HandleP2PConnected;
            P2PManager.instance.OnReady -= HandleP2PReady;
            P2PManager.instance.OnReliableData -= handleLoadMatch;
        }
    }
}
