using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// ── Response DTOs ──────────────────────────────────────────────────────────

[Serializable]
public class MatchResponseDto
{
    [SerializeField] private string id;
    [SerializeField] private string hostPlayer;

    public string Id { get => id; set => id = value; }
    public string HostPlayer { get => hostPlayer; set => hostPlayer = value; }
}

[Serializable]
public class MessageResponseDto
{
    [SerializeField] private string message;

    public string Message { get => message; set => message = value; }
}

public class MatchMakingService : MonoBehaviour
{
    private const string BaseUrl = "http://localhost:8080";
    private const string BasePath = "/api/player/match";

    // ── Request body ──────────────────────────────────────────────────────────

    [Serializable]
    private class PlayerRequestBody
    {
        public string player;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Sends a POST /find request to start matchmaking.</summary>
    public void FindMatch(Action<MatchResponseDto> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(PostRequest<MatchResponseDto>("/find", onSuccess, onError));
    }

    /// <summary>Sends a POST /cancel request to cancel matchmaking.</summary>
    public void CancelMatch(Action<MessageResponseDto> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(PostRequest<MessageResponseDto>("/cancel", onSuccess, onError));
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    private IEnumerator PostRequest<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
    {
        string url = BaseUrl + BasePath + endpoint;

        PlayerRequestBody body = new PlayerRequestBody
        {
            player = PlayerData.instance.player
        };

        string json = JsonUtility.ToJson(body);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log($"[MatchMakingService] {endpoint} success: {response}");
                
                try
                {
                    T result = JsonUtility.FromJson<T>(response);
                    onSuccess?.Invoke(result);
                }
                catch (Exception ex)
                {
                    string parseError = $"Failed to parse response: {ex.Message}";
                    Debug.LogError($"[MatchMakingService] {parseError}");
                    onError?.Invoke(parseError);
                }
            }
            else
            {
                string error = $"{request.responseCode} {request.error}";
                Debug.LogError($"[MatchMakingService] {endpoint} failed: {error}");
                onError?.Invoke(error);
            }
        }
    }
}
