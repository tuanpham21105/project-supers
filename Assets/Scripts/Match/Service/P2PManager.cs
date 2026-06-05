using System;
using Newtonsoft.Json;
using UnityEngine;

public class P2PManager : MonoBehaviour
{
    public static P2PManager instance;

    private UnityPeerJS.Peer _peer;
    private UnityPeerJS.Peer.IConnection _conn;

    // Events để các script khác lắng nghe
    public event Action OnReady;
    public event Action<string> OnReliableData;
    public event Action<string> OnUnreliableData;
    public event Action OnConnected; 
    public event Action OnDisconnected;
    public event Action<string> OnError;

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

    // ─────────────────────────────────────────────
    // Connect
    // ─────────────────────────────────────────────

    public void Init(string myId)
    {
        _peer = new UnityPeerJS.Peer("", myId, "", 0);

        _peer.OnOpen += () =>
        {
            Debug.Log("[P2P] Ready, my ID: " + myId);
            OnReady?.Invoke();
        };

        _peer.OnConnection += (conn) =>
        {
            RegisterConnection(conn);
        };

        _peer.OnDisconnected += () =>
        {
            Debug.Log("[P2P] Disconnected from server");
            OnDisconnected?.Invoke();
        };

        _peer.OnError += (err) =>
        {
            Debug.LogError("[P2P] Error: " + err);
            OnError?.Invoke(err);
        };
    }

    public void ConnectTo(string remoteId)
    {
        if (_peer == null)
        {
            Debug.LogError("[P2P] Peer not initialized. Call Init() first.");
            return;
        }

        Debug.Log("[P2P] Connecting to: " + remoteId);
        _peer.Connect(remoteId);
    }

    private void RegisterConnection(UnityPeerJS.Peer.IConnection conn)
    {
        _conn = conn;
        Debug.Log("[P2P] Connected to: " + conn.RemoteId);

        OnConnected?.Invoke();

        conn.OnReliableData += (data) =>
        {
            OnReliableData?.Invoke(data);
        };

        conn.OnUnreliableData += (data) =>
        {
            OnUnreliableData?.Invoke(data);
        };

        conn.OnClose += () =>
        {
            Debug.Log("[P2P] Connection closed: " + conn.RemoteId);
            _conn = null;
            OnDisconnected?.Invoke();
        };
    }

    // ─────────────────────────────────────────────
    // Send
    // ─────────────────────────────────────────────

    /// <summary>Gửi data quan trọng — guaranteed delivery</summary>
    public void SendData(string data)
    {
        if (_conn == null)
        {
            Debug.LogWarning("[P2P] SendData: no connection");
            return;
        }
        _conn.Send(data);
    }

    /// <summary>Gửi frame state — fast, có thể mất gói</summary>
    public void SendDataUnreliable(string data)
    {
        if (_conn == null)
        {
            Debug.LogWarning("[P2P] SendDataUnreliable: no connection");
            return;
        }
        _conn.SendUnreliable(data);
    }

    /// <summary>Gửi object dưới dạng JSON qua reliable</summary>
    public void SendJson<T>(T obj)
    {
        SendData(JsonConvert.SerializeObject(obj));
    }

    /// <summary>Gửi object dưới dạng JSON qua unreliable</summary>
    public void SendJsonUnreliable<T>(T obj)
    {
        SendDataUnreliable(JsonConvert.SerializeObject(obj));
    }

    // ─────────────────────────────────────────────
    // Disconnect
    // ─────────────────────────────────────────────

    /// <summary>Đóng connection hiện tại, giữ peer server connection</summary>
    public void DisconnectFromPeer()
    {
        if (_conn == null)
        {
            Debug.LogWarning("[P2P] DisconnectFromPeer: no connection to close");
            return;
        }

        Debug.Log("[P2P] Disconnecting from peer: " + _conn.RemoteId);
        _conn.Close();
        _conn = null;
    }

    /// <summary>Ngắt kết nối hoàn toàn khỏi PeerJS server</summary>
    public void Disconnect()
    {
        if (_peer == null)
        {
            Debug.LogWarning("[P2P] Disconnect: peer not initialized");
            return;
        }

        DisconnectFromPeer();

        Debug.Log("[P2P] Disconnecting from server");
        _peer.Disconnect();
    }

    /// <summary>Destroy peer hoàn toàn, dùng khi thoát game</summary>
    public void DestroyPeer()
    {
        if (_peer == null) return;

        DisconnectFromPeer();

        Debug.Log("[P2P] Destroying peer");
        _peer.Destroy();
        _peer = null;
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    public bool IsConnected => _conn != null;
    public bool IsInitialized => _peer != null;
    public string RemoteId => _conn?.RemoteId;

    void Update()
    {
        _peer?.Pump();
    }

    void OnDestroy()
    {
        DestroyPeer();
    }
}