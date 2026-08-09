using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Đọc file config.json từ StreamingAssets — có thể chỉnh sửa TRỰC TIẾP
/// sau khi build WebGL mà KHÔNG cần build lại project.
///
/// File nằm ở: Build/StreamingAssets/config.json (sau khi build)
///             Assets/StreamingAssets/config.json (trong Editor)
/// </summary>
[Serializable]
public class GameConfig
{
    public string baseUrl;
    public string baseRestSchema;
    public string baseWebSocketSchema;
}

public class ConfigLoader : MonoBehaviour
{
    public static GameConfig config;
    public static bool IsLoaded { get; private set; } = false;

    public static bool IsDeploymentBuild
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            return !Debug.isDebugBuild;
#endif
        }
    }

    public static event Action OnConfigLoaded;

    private const string CONFIG_FILE_NAME = "config.json";

    /// <summary>Gọi hàm này ở Loading Screen hoặc scene đầu tiên, TRƯỚC khi dùng config ở đâu khác</summary>
    public static IEnumerator Load()
    {
        if (IsLoaded)
        {
            OnConfigLoaded?.Invoke();
            yield break;
        }

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_NAME);

        using (UnityWebRequest req = UnityWebRequest.Get(path))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    config = JsonUtility.FromJson<GameConfig>(req.downloadHandler.text);
                    IsLoaded = true;
                    Debug.Log($"[ConfigLoader] Loaded thành công: {req.downloadHandler.text}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ConfigLoader] Lỗi parse JSON: {e.Message}");
                    UseDefaultConfig();
                }
            }
            else
            {
                Debug.LogError($"[ConfigLoader] Không load được config: {req.error}");
                UseDefaultConfig();
            }
        }

        OnConfigLoaded?.Invoke();
    }

    /// <summary>Phiên bản async của Load() — dùng khi cần await từ async method</summary>
    public static async Task LoadAsync()
    {
        if (IsLoaded)
        {
            OnConfigLoaded?.Invoke();
            return;
        }

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_NAME);

        using (UnityWebRequest req = UnityWebRequest.Get(path))
        {
            UnityWebRequestAsyncOperation op = req.SendWebRequest();

            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    config = JsonUtility.FromJson<GameConfig>(req.downloadHandler.text);
                    IsLoaded = true;
                    Debug.Log($"[ConfigLoader] Loaded thành công: {req.downloadHandler.text}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ConfigLoader] Lỗi parse JSON: {e.Message}");
                    UseDefaultConfig();
                }
            }
            else
            {
                Debug.LogError($"[ConfigLoader] Không load được config: {req.error}");
                UseDefaultConfig();
            }
        }

        OnConfigLoaded?.Invoke();
    }

    private static void UseDefaultConfig()
    {
        // Fallback nếu file config.json không tồn tại hoặc lỗi — tránh crash toàn game
        config = new GameConfig
        {
           baseUrl = "localhost:8080",
           baseRestSchema = "http://",
           baseWebSocketSchema = "ws://"
        };
        IsLoaded = true;
    }
}
