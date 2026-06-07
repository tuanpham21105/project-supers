using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Đặt script này vào GameObject trong LoadingScene.
/// Setup UI: ProgressBar (Image fillAmount), ProgressText (TMP), optional LoadingText
/// </summary>
public class LoadingSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI loadingLabel;

    [Header("Settings")]
    [SerializeField] private float minLoadTime = 0.5f;   // thời gian load tối thiểu (giây)
    [SerializeField] private float smoothSpeed  = 5f;     // tốc độ animate progress bar

    private float _displayProgress = 0f;

    void Start()
    {
        if (string.IsNullOrEmpty(SceneService.TargetScene))
        {
            Debug.LogError("[LoadingScene] TargetScene is empty! Use SceneService.LoadScene()");
            return;
        }

        StartCoroutine(LoadTargetScene(SceneService.TargetScene));
    }

    // ─────────────────────────────────────────────
    // Core
    // ─────────────────────────────────────────────

    private IEnumerator LoadTargetScene(string sceneName)
    {
        if (loadingLabel != null)
            loadingLabel.text = "Loading...";

        // Đảm bảo load tối thiểu minLoadTime để không flash quá nhanh
        float startTime = Time.realtimeSinceStartup;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // giữ lại ở loading screen cho đến khi sẵn sàng

        while (!op.isDone)
        {
            // Unity báo 0-0.9 khi loading, 1.0 khi done
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            // Smooth animate progress bar
            _displayProgress = Mathf.Lerp(_displayProgress, realProgress, Time.deltaTime * smoothSpeed);

            UpdateUI(_displayProgress);

            // Chờ minLoadTime và progress đạt 100%
            bool timeReady     = (Time.realtimeSinceStartup - startTime) >= minLoadTime;
            bool loadingReady  = op.progress >= 0.9f;

            if (timeReady && loadingReady)
            {
                // Animate progress bar lên 100% trước khi chuyển scene
                yield return StartCoroutine(AnimateToFull());
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator AnimateToFull()
    {
        while (_displayProgress < 0.99f)
        {
            _displayProgress = Mathf.Lerp(_displayProgress, 1f, Time.deltaTime * smoothSpeed);
            UpdateUI(_displayProgress);
            yield return null;
        }

        UpdateUI(1f);
    }

    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────

    private void UpdateUI(float progress)
    {
        if (progressBar != null)
            progressBar.fillAmount = progress;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
    }
}
