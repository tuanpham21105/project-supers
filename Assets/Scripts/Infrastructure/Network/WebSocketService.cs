using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// ── Message Class Definitions ───────────────────────────────────────────────

[Serializable]
public class Message
{
    [SerializeField] private string type;
    [SerializeField] private string sender;
    [SerializeField] private string receiver;
    [SerializeField] private string matchId;
    [SerializeField] private object value;

    public Message() { }

    public string Type { get => type; set => type = value; }
    public string Sender { get => sender; set => sender = value; }
    public string Receiver { get => receiver; set => receiver = value; }
    public string MatchId { get => matchId; set => matchId = value; }
    public object Value { get => value; set => this.value = value; }
}

// ── WebSocket Service ────────────────────────────────────────────────────────

public class WebSocketService : MonoBehaviour
{
    private const string BaseUrl = "ws://localhost:8080/ws/";

    private ClientWebSocket _webSocket;
    private CancellationTokenSource _cts;
    private readonly ConcurrentQueue<string> _receiveQueue = new ConcurrentQueue<string>();

    // Public Events for subscription
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<Message> OnMessageReceived;
    public event Action<string> OnRawMessageReceived;
    public event Action<string> OnError;

    private void Update()
    {
        // Process queued messages on the main Unity thread
        while (_receiveQueue.TryDequeue(out string messageJson))
        {
            OnRawMessageReceived?.Invoke(messageJson);
            
            try
            {
                Message message = DeserializeMessage(messageJson);
                if (message != null)
                {
                    OnMessageReceived?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WebSocketService] Failed to deserialize message: {ex.Message}");
            }
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    // ── Public Connection API ──────────────────────────────────────────────────

    /// <summary>
    /// Connect to the WebSocket server using the player name from PlayerData.
    /// </summary>
    public async void Connect()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            Debug.LogWarning("[WebSocketService] Already connected.");
            return;
        }

        if (PlayerData.instance == null || string.IsNullOrEmpty(PlayerData.instance.player))
        {
            string errorMsg = "Cannot connect: PlayerData instance or player name is null/empty.";
            Debug.LogError($"[WebSocketService] {errorMsg}");
            OnError?.Invoke(errorMsg);
            return;
        }

        string url = BaseUrl + Uri.EscapeDataString(PlayerData.instance.player);
        Debug.Log($"[WebSocketService] Connecting to {url}...");

        _webSocket = new ClientWebSocket();
        _cts = new CancellationTokenSource();

        try
        {
            await _webSocket.ConnectAsync(new Uri(url), _cts.Token);
            Debug.Log("[WebSocketService] Successfully connected!");
            OnConnected?.Invoke();

            // Start listening for incoming messages
            _ = ReceiveLoop(_webSocket, _cts.Token);
        }
        catch (Exception ex)
        {
            string errorMsg = $"Connection failed: {ex.Message}";
            Debug.LogError($"[WebSocketService] {errorMsg}");
            OnError?.Invoke(errorMsg);
            Cleanup();
        }
    }

    /// <summary>
    /// Disconnect from the WebSocket server.
    /// </summary>
    public async void Disconnect()
    {
        if (_webSocket == null) return;

        Debug.Log("[WebSocketService] Disconnecting...");
        _cts?.Cancel();

        if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[WebSocketService] CloseAsync error: {ex.Message}");
            }
        }

        Cleanup();
        OnDisconnected?.Invoke();
    }

    /// <summary>
    /// Send a Message object to the WebSocket server.
    /// </summary>
    public async void Send(Message message)
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("[WebSocketService] Cannot send message: Connection is not open.");
            return;
        }

        try
        {
            string json = SerializeMessage(message);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[WebSocketService] Send failed: {ex.Message}");
            OnError?.Invoke($"Send failed: {ex.Message}");
        }
    }

    // ── Internal Helpers ───────────────────────────────────────────────────────

    private async Task ReceiveLoop(ClientWebSocket socket, CancellationToken token)
    {
        byte[] buffer = new byte[4096];
        StringBuilder messageBuilder = new StringBuilder();

        try
        {
            while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("[WebSocketService] Server closed the connection.");
                    Disconnect();
                    break;
                }

                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                if (result.EndOfMessage)
                {
                    string messageJson = messageBuilder.ToString();
                    messageBuilder.Clear();
                    _receiveQueue.Enqueue(messageJson);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal when cancelling/disconnecting
        }
        catch (Exception ex)
        {
            Debug.LogError($"[WebSocketService] Receive loop error: {ex.Message}");
            OnError?.Invoke($"Receive error: {ex.Message}");
        }
    }

    private void Cleanup()
    {
        _webSocket?.Dispose();
        _webSocket = null;

        _cts?.Dispose();
        _cts = null;
    }

    // ── JSON Serialization & Deserialization ───────────────────────────────────

    private string SerializeMessage(Message message)
    {
        string valStr = GetValueJsonString(message.Value);
        return $"{{\"type\":\"{Escape(message.Type)}\",\"sender\":\"{Escape(message.Sender)}\",\"receiver\":\"{Escape(message.Receiver)}\",\"matchId\":\"{Escape(message.MatchId)}\",\"value\":{valStr}}}";
    }

    private string GetValueJsonString(object val)
    {
        if (val == null) return "null";
        if (val is string s)
        {
            string trimmed = s.Trim();
            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) || (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                return trimmed; // Already JSON formatted string
            }
            return $"\"{Escape(s)}\"";
        }
        if (val is bool b) return b ? "true" : "false";
        if (val is int || val is float || val is double || val is long) return val.ToString();

        return JsonUtility.ToJson(val);
    }

    private string Escape(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    private Message DeserializeMessage(string json)
    {
        string type = ExtractJsonField(json, "type");
        string sender = ExtractJsonField(json, "sender");
        string receiver = ExtractJsonField(json, "receiver");
        string matchId = ExtractJsonField(json, "matchId");
        string rawValue = ExtractRawValueField(json, "value");

        object val = rawValue;
        
        // Simple heuristic to unwrap simple values if they are primitive representations
        if (!string.IsNullOrEmpty(rawValue))
        {
            string trimmed = rawValue.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\"") && trimmed.Length >= 2)
            {
                val = trimmed.Substring(1, trimmed.Length - 2).Replace("\\\"", "\"").Replace("\\\\", "\\");
            }
            else if (trimmed == "true") val = true;
            else if (trimmed == "false") val = false;
            else if (int.TryParse(trimmed, out int iVal)) val = iVal;
            else if (float.TryParse(trimmed, out float fVal)) val = fVal;
        }

        return new Message
        {
            Type = type,
            Sender = sender,
            Receiver = receiver,
            MatchId = matchId,
            Value = val
        };
    }

    private string ExtractJsonField(string json, string key)
    {
        string pattern = $"\"{key}\"\\s*:\\s*\"(.*?)\"(?:,|\\s*\\}})";
        var match = System.Text.RegularExpressions.Regex.Match(json, pattern);
        return match.Success ? match.Groups[1].Value : null;
    }

    private string ExtractRawValueField(string json, string key)
    {
        int keyIndex = json.IndexOf($"\"{key}\"");
        if (keyIndex == -1) return null;

        int colonIndex = json.IndexOf(':', keyIndex);
        if (colonIndex == -1) return null;

        int startIndex = colonIndex + 1;
        while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
        {
            startIndex++;
        }

        if (startIndex >= json.Length) return null;

        // String value
        if (json[startIndex] == '"')
        {
            int endIndex = json.IndexOf('"', startIndex + 1);
            while (endIndex != -1 && json[endIndex - 1] == '\\')
            {
                endIndex = json.IndexOf('"', endIndex + 1);
            }
            if (endIndex == -1) return null;
            return json.Substring(startIndex, endIndex - startIndex + 1);
        }

        // Object or array
        if (json[startIndex] == '{' || json[startIndex] == '[')
        {
            char openChar = json[startIndex];
            char closeChar = openChar == '{' ? '}' : ']';
            int braceCount = 1;
            int currentIndex = startIndex + 1;

            while (currentIndex < json.Length && braceCount > 0)
            {
                if (json[currentIndex] == '"')
                {
                    currentIndex++;
                    while (currentIndex < json.Length && json[currentIndex] != '"')
                    {
                        if (json[currentIndex] == '\\') currentIndex++;
                        currentIndex++;
                    }
                }
                else if (json[currentIndex] == openChar)
                {
                    braceCount++;
                }
                else if (json[currentIndex] == closeChar)
                {
                    braceCount--;
                }
                currentIndex++;
            }

            if (braceCount == 0)
            {
                return json.Substring(startIndex, currentIndex - startIndex);
            }
            return null;
        }

        // Primitive values (number, boolean, null)
        int nextComma = json.IndexOf(',', startIndex);
        int nextCloseBrace = json.IndexOf('}', startIndex);
        int endIndexToUse = json.Length;
        if (nextComma != -1) endIndexToUse = Mathf.Min(endIndexToUse, nextComma);
        if (nextCloseBrace != -1) endIndexToUse = Mathf.Min(endIndexToUse, nextCloseBrace);

        return json.Substring(startIndex, endIndexToUse - startIndex).Trim();
    }
}
