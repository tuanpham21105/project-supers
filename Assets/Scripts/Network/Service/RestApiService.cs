using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class RestApiService : MonoBehaviour
{
    [SerializeField] private NetworkDataSO networkData;

    [SerializeField] private string baseUrl = "";

    public static RestApiService instance;

    private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore
    };

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

        baseUrl = networkData.BaseRestSchema() + networkData.BaseUrl();
    }
    
    // ─────────────────────────────────────────────
    // Core
    // ─────────────────────────────────────────────

    public IEnumerator SendRequestWithJwt<T>(
        string method,
        string path,
        object body,
        Dictionary<string, string> headers,
        Action<T> onSuccess,
        Action<long, string> onError)
    {
        yield return SendRequestWithJwtInternal<T>(method, path, body, headers, onSuccess, onError, false);
    }

    private IEnumerator SendRequestWithJwtInternal<T>(
        string method,
        string path,
        object body,
        Dictionary<string, string> headers,
        Action<T> onSuccess,
        Action<long, string> onError,
        bool isRetry)
    {
        if (headers == null)
        {
            headers = new Dictionary<string, string>();
        }
        else
        {
            headers = new Dictionary<string, string>(headers);
        }

        string token = CookieService.Get("accessToken");
        if (!string.IsNullOrEmpty(token))
        {
            headers["Authorization"] = "Bearer " + token;
        }
        else
        {
            headers.Remove("Authorization");
        }

        yield return SendRequest<T>(
            method,
            path,
            body,
            headers,
            onSuccess,
            (statusCode, errMsg) =>
            {
                if (statusCode == 401 && !isRetry)
                {
                    Debug.LogWarning("[REST] 401 Unauthorized. Attempting to refresh access token...");
                    if (PlayerAuthService.instance != null)
                    {
                        PlayerAuthService.instance.RefreshAccessToken(
                            (refreshResponse) =>
                            {
                                Debug.Log("[REST] Token refresh successful. Retrying original request...");
                                StartCoroutine(SendRequestWithJwtInternal<T>(
                                    method,
                                    path,
                                    body,
                                    headers,
                                    onSuccess,
                                    onError,
                                    true
                                ));
                            },
                            (refreshCode, refreshMsg) =>
                            {
                                Debug.LogError($"[REST] Token refresh failed ({refreshCode}): {refreshMsg}. Logging out...");
                                PlayerAuthService.instance.Logout();
                                onError?.Invoke(statusCode, errMsg);
                            }
                        );
                    }
                    else
                    {
                        onError?.Invoke(statusCode, errMsg);
                    }
                }
                else
                {
                    onError?.Invoke(statusCode, errMsg);
                }
            }
        );
    }

    public IEnumerator SendRequest<T>(
        string method,
        string path,
        object body,
        Dictionary<string, string> headers,
        Action<T> onSuccess,
        Action<long, string> onError)
    {
        string url = baseUrl + path;
        byte[] bodyBytes = null;

        if (body != null)
        {
            string json = JsonConvert.SerializeObject(body, _jsonSettings);
            bodyBytes = Encoding.UTF8.GetBytes(json);
        }

        using (UnityWebRequest req = new UnityWebRequest(url, method))
        {
            // Body
            if (bodyBytes != null)
                req.uploadHandler = new UploadHandlerRaw(bodyBytes);

            req.downloadHandler = new DownloadHandlerBuffer();

            // Headers mặc định
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept",       "application/json");

            // Headers tùy chỉnh
            if (headers != null)
                foreach (var h in headers)
                    req.SetRequestHeader(h.Key, h.Value);

            yield return req.SendWebRequest();

            // Error
            if (req.result != UnityWebRequest.Result.Success)
            {
                string errorResponseText = req.downloadHandler.text;
                string finalErrorMsg = req.error;

                if (!string.IsNullOrEmpty(errorResponseText))
                {
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<MessageResponse<string>>(errorResponseText, _jsonSettings);
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.message))
                        {
                            finalErrorMsg = errorResponse.message;
                        }
                    }
                    catch
                    {
                        // If parsing fails, stick with the default req.error
                    }
                }

                Debug.LogWarning($"[REST] {method} {url} → {req.responseCode} | Error: {finalErrorMsg}");

                onError?.Invoke(req.responseCode, finalErrorMsg);
                yield break;
            }

            // Success
            string responseText = req.downloadHandler.text;
            Debug.Log($"[REST] {method} {url} → {req.responseCode}");

            try
            {
                T result = JsonConvert.DeserializeObject<T>(responseText, _jsonSettings);
                onSuccess?.Invoke(result);
            }
            catch (Exception e)
            {
                string errMsg = $"[REST] Parse error: {e.Message}\nRaw: {responseText}";
                Debug.LogError(errMsg);
                onError?.Invoke(200, errMsg);
            }
        }
    }
}
