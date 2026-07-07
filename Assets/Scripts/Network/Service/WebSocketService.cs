using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class WebSocketService : MonoBehaviour
{
    [SerializeField] private NetworkDataSO networkData;

    [SerializeField] private string baseUrl = "";
    private string basePath = "/ws/";
    private WebSocket _ws;

    public static WebSocketService instance;

    public event Action<WsMessage> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;

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

        baseUrl = networkData.BaseWebSocketSchema() + networkData.BaseUrl();
    }

    // ─────────────────────────────────────────────
    // Connect / Disconnect
    // ─────────────────────────────────────────────

    public async Task Connect()
    {
        string token = CookieService.Get("accessToken");
        
        string url = baseUrl + basePath + PlayerData.instance.username + "?token=" + token;

        // Thêm headers vào Dictionary
        Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + token}
        };

        _ws = new WebSocket(url, headers);

        _ws.OnOpen += () =>
        {
            Debug.Log("[WS] Connected");
            OnConnected?.Invoke();
        };

        _ws.OnClose += (code) =>
        {
            Debug.Log("[WS] Closed: " + code);
            OnDisconnected?.Invoke();
        };

        _ws.OnError += (err) => Debug.LogError("[WS] Error: " + err);

        _ws.OnMessage += (bytes) =>
        {
            try
            {
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                WsMessage msg = JsonConvert.DeserializeObject<WsMessage>(json);
                Debug.Log($"[WS] Message received: {json}");
                Debug.Log($"[WS] Message received - Type: {msg.type}");
                OnMessageReceived?.Invoke(msg);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        };

        await _ws.Connect();
    }

    public async void Disconnect()
    {
        if (_ws != null)
            await _ws.Close();
    }

    // ─────────────────────────────────────────────
    // Send
    // ─────────────────────────────────────────────

    public async void Send(WsMessage message)
    {
        if (_ws == null || _ws.State != WebSocketState.Open)
        {
            Debug.LogWarning("[WS] Send failed: not connected");
            return;
        }

        await _ws.SendText(JsonUtility.ToJson(message));
    }

    // ─────────────────────────────────────────────
    // Loop
    // ─────────────────────────────────────────────

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _ws?.DispatchMessageQueue();
#endif
    }

    void OnDestroy() => Disconnect();
}