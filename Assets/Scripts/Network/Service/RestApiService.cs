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
    // Public Methods
    // ─────────────────────────────────────────────

    public void Get<T>(
        string path,
        Action<T> onSuccess,
        Action<string> onError = null,
        Dictionary<string, string> headers = null)
    {
        StartCoroutine(SendRequest<T>(
            method:    "GET",
            path:      path,
            body:      null,
            headers:   headers,
            onSuccess: onSuccess,
            onError:   onError
        ));
    }

    public void Post<T>(
        string path,
        object body,
        Action<T> onSuccess,
        Action<string> onError = null,
        Dictionary<string, string> headers = null)
    {
        StartCoroutine(SendRequest<T>(
            method:    "POST",
            path:      path,
            body:      body,
            headers:   headers,
            onSuccess: onSuccess,
            onError:   onError
        ));
    }

    public void Put<T>(
        string path,
        object body,
        Action<T> onSuccess,
        Action<string> onError = null,
        Dictionary<string, string> headers = null)
    {
        StartCoroutine(SendRequest<T>(
            method:    "PUT",
            path:      path,
            body:      body,
            headers:   headers,
            onSuccess: onSuccess,
            onError:   onError
        ));
    }

    public void Patch<T>(
        string path,
        object body,
        Action<T> onSuccess,
        Action<string> onError = null,
        Dictionary<string, string> headers = null)
    {
        StartCoroutine(SendRequest<T>(
            method:    "PATCH",
            path:      path,
            body:      body,
            headers:   headers,
            onSuccess: onSuccess,
            onError:   onError
        ));
    }

    public void Delete<T>(
        string path,
        Action<T> onSuccess,
        Action<string> onError = null,
        Dictionary<string, string> headers = null,
        object body = null)
    {
        StartCoroutine(SendRequest<T>(
            method:    "DELETE",
            path:      path,
            body:      body,
            headers:   headers,
            onSuccess: onSuccess,
            onError:   onError
        ));
    }

    // ─────────────────────────────────────────────
    // Core
    // ─────────────────────────────────────────────

    private IEnumerator SendRequest<T>(
        string method,
        string path,
        object body,
        Dictionary<string, string> headers,
        Action<T> onSuccess,
        Action<string> onError)
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
                string errMsg = $"[REST] {method} {url} → {req.responseCode} {req.error}";
                Debug.LogError(errMsg);
                onError?.Invoke(errMsg);
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
                onError?.Invoke(errMsg);
            }
        }
    }
}
