using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.WebRTC;
using UnityEngine;

public class PeerConnectionManager : MonoBehaviour
{
    public static PeerConnectionManager Instance;

    private RTCPeerConnection _peerConnection;

    private RTCDataChannel _reliableDataChannel;
    private RTCDataChannel _unreliableDataChannel;

    public bool IsConnected =>
        _peerConnection != null 
        &&
        _peerConnection.ConnectionState ==RTCPeerConnectionState.Connected;

    public event Action<string> OnSendOffer;
    public event Action<string> OnSendAnswer;
    public event Action<IceCandidateData> OnSendIceCandidate;

    public event Action<Packet> OnMessageReceived;
    public event Action<RTCPeerConnectionState> OnConnectionStateChanged;

    private RTCConfiguration _config = new RTCConfiguration
    {
        iceServers = new[]
        {
            new RTCIceServer
            {
                urls = new[]
                {
                    "stun:stun.l.google.com:19302"
                }
            }
        }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region CREATE CONNECTION

    public void CreateConnection(bool isHost)
    {
        CloseConnection();

        _peerConnection =
            new RTCPeerConnection(ref _config);

        RegisterPeerCallbacks();

        if (isHost)
        {
            CreateDataChannel();
        }

        Debug.Log("[WebRTC] PeerConnection created");
    }

    #endregion

    #region CALLBACKS

    private void RegisterPeerCallbacks()
    {
        _peerConnection.OnIceCandidate = candidate =>
        {
            if (candidate == null) return;

            IceCandidateData data =
                new IceCandidateData
                {
                    candidate = candidate.Candidate,
                    sdpMid = candidate.SdpMid,
                    sdpMLineIndex =
                        candidate.SdpMLineIndex ?? 0
                };

            Debug.Log("[WebRTC] ICE generated");

            OnSendIceCandidate?.Invoke(data);
        };

        _peerConnection.OnConnectionStateChange =
            state =>
            {
                Debug.Log(
                    $"[WebRTC] Connection state: {state}");

                OnConnectionStateChanged?.Invoke(state);
            };

        _peerConnection.OnDataChannel = channel =>
        {
            Debug.Log($"[WebRTC] DataChannel received: {channel.Label}");

            RegisterDataChannel(channel);
        };
    }

    #endregion

    #region DATA CHANNEL

    private void CreateDataChannel()
    {
        RTCDataChannelInit reliableInit = new RTCDataChannelInit
        {
            ordered = true
        };

        _reliableDataChannel = _peerConnection.CreateDataChannel("reliable_game_events", reliableInit);
        RegisterDataChannel(_reliableDataChannel);

        RTCDataChannelInit unreliableInit = new RTCDataChannelInit
        {
            ordered = false,
            maxRetransmits = 0
        };

        _unreliableDataChannel = _peerConnection.CreateDataChannel("unreliable_game_states", unreliableInit);
        RegisterDataChannel(_unreliableDataChannel);

        Debug.Log("[WebRTC] DataChannels created");
    }

    private void RegisterDataChannel(RTCDataChannel channel)
    {
        if (channel.Label == "reliable_game_events" || channel.Label == "game")
        {
            _reliableDataChannel = channel;
        }
        else if (channel.Label == "unreliable_game_states")
        {
            _unreliableDataChannel = channel;
        }

        channel.OnOpen = () =>
        {
            Debug.Log($"[WebRTC] DataChannel open: {channel.Label}");
        };

        channel.OnClose = () =>
        {
            Debug.Log($"[WebRTC] DataChannel closed: {channel.Label}");
        };

        channel.OnMessage = bytes =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Packet packet = ParsePacket(message);
            if (packet != null)
            {
                OnMessageReceived?.Invoke(packet);
            }
        };
    }

    private Packet ParsePacket(string json)
    {
        try
        {
            Packet basePacket = JsonUtility.FromJson<Packet>(json);
            if (basePacket == null) return null;

            switch (basePacket.type)
            {
                case "FLYING_INTERRUPTED":
                    return JsonUtility.FromJson<FlyingInterruptedEventPacket>(json);
                case "STATES":
                    return JsonUtility.FromJson<StatesPacket>(json);
                case "ANIMATION":
                    return JsonUtility.FromJson<AnimationEventPacket>(json);
                case "INPUT":
                    return JsonUtility.FromJson<ActionEventPacket>(json);
                default:
                    return basePacket;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebRTC] Failed to parse packet: {e.Message}");
            return null;
        }
    }

    #endregion

    #region OFFER

    public async Task CreateOffer()
    {
        var op = _peerConnection.CreateOffer();

        while (!op.IsDone)
        {
            await Task.Yield();
        }

        if (op.IsError)
        {
            Debug.LogError(op.Error.message);
            return;
        }

        RTCSessionDescription offer = op.Desc;

        var setLocalOp =
            _peerConnection.SetLocalDescription(ref offer);

        while (!setLocalOp.IsDone)
        {
            await Task.Yield();
        }

        if (setLocalOp.IsError)
        {
            Debug.LogError(setLocalOp.Error.message);
            return;
        }

        Debug.Log("[WebRTC] Offer created");

        OnSendOffer?.Invoke(offer.sdp);
    }

    public async Task ReceiveOffer(string sdp)
    {
        RTCSessionDescription offer =
            new RTCSessionDescription
            {
                type = RTCSdpType.Offer,
                sdp = sdp
            };

        var op =
            _peerConnection.SetRemoteDescription(
                ref offer);

        while (!op.IsDone)
        {
            await Task.Yield();
        }

        if (op.IsError)
        {
            Debug.LogError(op.Error.message);
            return;
        }

        await CreateAnswer();
    }

    #endregion

    #region ANSWER

    public async Task CreateAnswer()
    {
        var op = _peerConnection.CreateAnswer();

        while (!op.IsDone)
        {
            await Task.Yield();
        }

        if (op.IsError)
        {
            Debug.LogError(op.Error.message);
            return;
        }

        RTCSessionDescription answer = op.Desc;

        var setLocalOp =
            _peerConnection.SetLocalDescription(ref answer);

        while (!setLocalOp.IsDone)
        {
            await Task.Yield();
        }

        if (setLocalOp.IsError)
        {
            Debug.LogError(setLocalOp.Error.message);
            return;
        }

        Debug.Log("[WebRTC] Answer created");

        OnSendAnswer?.Invoke(answer.sdp);
    }

    public async Task ReceiveAnswer(string sdp)
    {
        RTCSessionDescription answer =
            new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = sdp
            };

        var op =
            _peerConnection.SetRemoteDescription(
                ref answer);

        while (!op.IsDone)
        {
            await Task.Yield();
        }

        if (op.IsError)
        {
            Debug.LogError(op.Error.message);
        }
    }

    #endregion

    #region ICE

    public void AddIceCandidate(
        IceCandidateData data)
    {
        RTCIceCandidateInit init =
            new RTCIceCandidateInit
            {
                candidate = data.candidate,
                sdpMid = data.sdpMid,
                sdpMLineIndex = data.sdpMLineIndex
            };

        RTCIceCandidate candidate =
            new RTCIceCandidate(init);

        _peerConnection.AddIceCandidate(candidate);

        Debug.Log("[WebRTC] ICE candidate added");
    }

    #endregion

    #region SEND / RECEIVE

    public void Send(Packet packet, bool reliable = true)
    {
        RTCDataChannel channel = reliable ? _reliableDataChannel : _unreliableDataChannel;

        if (channel == null || channel.ReadyState != RTCDataChannelState.Open)
            return;

        string message = JsonUtility.ToJson(packet);
        byte[] bytes = Encoding.UTF8.GetBytes(message);

        channel.Send(bytes);
    }

    #endregion

    #region CLOSE

    public void CloseConnection()
    {
        if (_reliableDataChannel != null)
        {
            _reliableDataChannel.Close();
            _reliableDataChannel.Dispose();
            _reliableDataChannel = null;
        }

        if (_unreliableDataChannel != null)
        {
            _unreliableDataChannel.Close();
            _unreliableDataChannel.Dispose();
            _unreliableDataChannel = null;
        }

        if (_peerConnection != null)
        {
            _peerConnection.Close();
            _peerConnection.Dispose();
            _peerConnection = null;
        }
    }

    private void OnDestroy()
    {
        CloseConnection();
    }

    #endregion
}

[Serializable]
public class IceCandidateData
{
    public string candidate;

    public string sdpMid;

    public int sdpMLineIndex;
}
