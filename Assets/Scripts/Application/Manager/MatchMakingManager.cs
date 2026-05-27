using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.WebRTC;

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

    [SerializeField] private WsMessageType waitingForMessage;

    void Start()
    {
        matchMakingService = GetComponent<MatchMakingService>();
        webSocketService = GetComponent<WebSocketService>();
    }

    /// <summary>
    /// Starts matchmaking by invoking the find match API and establishing a WebSocket connection.
    /// Also sets up the event listeners for WebSocket messages.
    /// </summary>
    [ProButton]
    public void StartMatchMaking()
    {
        if (matchMakingService == null || webSocketService == null)
        {
            Debug.LogError("[MatchMakingManager] MatchMakingService or WebSocketService dependency is missing.");
            return;
        }

        ResetMatchData();

        waitingForMessage = WsMessageType.MATCH_READY;

        // Connect to WebSocket service
        webSocketService.Connect();

        webSocketService.OnMessageReceived += HandleMessageReceived;
        PeerConnectionManager.Instance.OnSendOffer -= HandleSendOffer;
        PeerConnectionManager.Instance.OnSendAnswer -= HandleSendAnswer;
        PeerConnectionManager.Instance.OnSendIceCandidate -= HandleSendIceCandidate;

        PeerConnectionManager.Instance.OnSendOffer += HandleSendOffer;
        PeerConnectionManager.Instance.OnSendAnswer += HandleSendAnswer;
        PeerConnectionManager.Instance.OnSendIceCandidate += HandleSendIceCandidate;

        PeerConnectionManager.Instance.OnConnectionStateChanged += HandleConnected;

        // Call findMatch API
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

    void ResetMatchData()
    {
        MatchData.matchId = "";
        MatchData.hostPlayer = "";
        MatchData.players = new List<string>();
    }

    /// <summary>
    /// Cancels the matchmaking process by calling the cancel match API and disconnecting from WebSocket.
    /// </summary>
    [ProButton]
    public void CancelMatchMaking()
    {
        if (matchMakingService == null || webSocketService == null)
        {
            Debug.LogError("[MatchMakingManager] MatchMakingService or WebSocketService dependency is missing.");
            return;
        }

        // Call cancelMatch API
        matchMakingService.CancelMatch(
            onSuccess: (messageResponse) =>
            {
                Debug.Log($"[MatchMakingManager] CancelMatch succeeded: {messageResponse.Message}");
            },
            onError: (error) =>
            {
                Debug.LogError($"[MatchMakingManager] CancelMatch failed: {error}");
            }
        );

        // Disconnect WebSocket
        webSocketService.Disconnect();

        // Unsubscribe from events to clean up
        webSocketService.OnMessageReceived -= HandleMessageReceived;

        if (PeerConnectionManager.Instance != null)
        {
            PeerConnectionManager.Instance.OnSendOffer -= HandleSendOffer;
            PeerConnectionManager.Instance.OnSendAnswer -= HandleSendAnswer;
            PeerConnectionManager.Instance.OnSendIceCandidate -= HandleSendIceCandidate;
        }
    }

    private void OnDestroy()
    {
        // Clean up subscription to prevent memory leaks
        if (webSocketService != null)
        {
            webSocketService.OnMessageReceived -= HandleMessageReceived;
        }

        if (PeerConnectionManager.Instance != null)
        {
            PeerConnectionManager.Instance.OnSendOffer -= HandleSendOffer;
            PeerConnectionManager.Instance.OnSendAnswer -= HandleSendAnswer;
            PeerConnectionManager.Instance.OnSendIceCandidate -= HandleSendIceCandidate;
        }
    }

    private async void HandleMessageReceived(Message message)
    {
        if (message == null) return;

        // ICE candidates can arrive multiple times and anytime after offer/answer.
        // We shouldn't block them with the strict waitingForMessage check.
        if (message.Type == WsMessageType.ICE_CANDIDATE.ToString())
        {
            // Both host and client should receive ICE candidates
            IceCandidateData data = null;
            
            // Check if it's already the right type, or parse it if it's a JSON object
            if (message.Value is IceCandidateData)
            {
                data = message.Value as IceCandidateData;
            }
            else if (message.Value != null)
            {
                // In case your JSON deserializer returns a generic object/JObject
                try {
                    data = JsonUtility.FromJson<IceCandidateData>(message.Value.ToString());
                } catch { }
            }

            if (data != null)
            {
                PeerConnectionManager.Instance.AddIceCandidate(data);
            }
            return; // We process and return, keeping the current waitingForMessage state intact
        }

        if (message.Type != waitingForMessage.ToString()) return;

        // Print out when receive ws message
        Debug.Log($"[MatchMakingManager] WS Message Received: Type={message.Type}, Sender={message.Sender}, Value={message.Value}");

        if (message.Type.CompareTo(WsMessageType.MATCH_READY.ToString()) == 0)
        {
            HandleMatchReadyMessage(message);

            if (MatchData.hostPlayer == PlayerData.instance.player)
            {
                PeerConnectionManager.Instance.CreateConnection(true);
                await PeerConnectionManager.Instance.CreateOffer();
                waitingForMessage  = WsMessageType.ANSWER;
            }
            else
            {
                PeerConnectionManager.Instance.CreateConnection(false);
                waitingForMessage = WsMessageType.OFFER;
            }
        }
        else if (message.Type.CompareTo(WsMessageType.OFFER.ToString()) == 0)
        {
            if (MatchData.hostPlayer != PlayerData.instance.player) {
                string sdp = message.Value as string;

                await PeerConnectionManager.Instance.ReceiveOffer(sdp);

                waitingForMessage = WsMessageType.ANSWER; // Usually wait for nothing or ICE candidates next, but ICE is handled above
            }
        }
        else if (message.Type.CompareTo(WsMessageType.ANSWER.ToString()) == 0) 
        {
            if (MatchData.hostPlayer == PlayerData.instance.player) {
                string sdp = message.Value as string;

                await PeerConnectionManager.Instance.ReceiveAnswer(sdp);

                waitingForMessage = WsMessageType.none;
            }
        }
    }

    public void HandleMatchReadyMessage(Message message)
    {
        ReadyMatchDto readyMatchDto = message.Value as ReadyMatchDto;
        
        MatchData.matchId = readyMatchDto.id;
        MatchData.hostPlayer = readyMatchDto.hostPlayer;
        MatchData.players.Add(readyMatchDto.hostPlayer);
        MatchData.players.Add(readyMatchDto.clientPlayer);
    }

    private void HandleSendOffer(string offer)
    {
        Message message = new Message();
        message.Type = WsMessageType.OFFER.ToString();
        message.Sender = PlayerData.instance.player;
        message.Receiver = "";
        message.MatchId = MatchData.matchId;
        message.Value = offer;

        webSocketService.Send(message);
    }

    private void HandleSendAnswer(string answer)
    {
        Message message = new Message();
        message.Type = WsMessageType.ANSWER.ToString();
        message.Sender = PlayerData.instance.player;
        message.Receiver = "";
        message.MatchId = MatchData.matchId;
        message.Value = answer;

        webSocketService.Send(message);
    }

    private void HandleSendIceCandidate(IceCandidateData data)
    {
        Message message = new Message();
        message.Type = WsMessageType.ICE_CANDIDATE.ToString();
        message.Sender = PlayerData.instance.player;
        message.Receiver = "";
        message.MatchId = MatchData.matchId;
        message.Value = data;

        webSocketService.Send(message);
    }

    private void HandleConnected(RTCPeerConnectionState state)
    {
        if (state == RTCPeerConnectionState.Connected)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
