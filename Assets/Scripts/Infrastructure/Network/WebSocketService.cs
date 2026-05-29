using System;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class WebSocketService : MonoBehaviour
{
    private string baseUrl = "ws://localhost:8080";
    private string basePath = "/ws/";
    private WebSocket _ws;

    public event Action<Message> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;

    // ─────────────────────────────────────────────
    // Connect / Disconnect
    // ─────────────────────────────────────────────

    public async Task Connect()
    {
        string url = baseUrl + basePath + PlayerData.instance.player;
        _ws = new WebSocket(url);

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
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            Message msg = JsonConvert.DeserializeObject<Message>(json);
            Debug.Log($"[WS] Message received: {json}");
            Debug.Log($"[WS] Message received - Type: {msg.type}, MatchId: {msg.matchId}, Value: {msg.value.ToString()}");
            OnMessageReceived?.Invoke(msg);
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

    public async void Send(Message message)
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