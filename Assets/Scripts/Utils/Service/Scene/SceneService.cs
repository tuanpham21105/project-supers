using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Đặt GameObject này ở scene đầu tiên.
/// Gọi SceneService.instance để dùng ở bất kỳ đâu.
/// </summary>
public class SceneService : MonoBehaviour
{
    public static SceneService instance;

    [Header("Scene Names")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    // Scene đích được lưu lại để LoadingSceneController đọc
    public static string TargetScene { get; private set; }

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
    // Public API
    // ─────────────────────────────────────────────

    /// <summary>Load scene mới qua Loading Screen</summary>
    public void LoadScene(string sceneName)
    {
        TargetScene = sceneName;
        SceneManager.LoadScene(loadingSceneName);
    }

    /// <summary>Reload scene hiện tại qua Loading Screen</summary>
    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>Load scene trực tiếp không qua Loading Screen</summary>
    public void LoadSceneDirect(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
