using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class TransparentRenderCapture : MonoBehaviour
{
    public static TransparentRenderCapture instance;

    [SerializeField] private Camera captureCamera;
    [SerializeField] private string targetLayerName = "CaptureLayer";
    [SerializeField] private int resolution = 256;

    [SerializeField] private Material _projectorMaterial;
    [SerializeField] private EmblemCanvasUiController canvas;

    private RenderTexture _rt;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        captureCamera.enabled = false;
        captureCamera.gameObject.SetActive(false);
    }

    public Material Capture(Emblem emblem)
    {
        captureCamera.enabled = true;
        captureCamera.gameObject.SetActive(true);

        canvas.ApplyEmblem(emblem);

        // ─── Tạo RenderTexture với alpha (như đã làm ở bước trước) ───
        _rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        _rt.Create();

        captureCamera.clearFlags = CameraClearFlags.SolidColor;
        captureCamera.backgroundColor = new Color(0, 0, 0, 0);
        captureCamera.targetTexture = _rt;
        captureCamera.Render();

        captureCamera.enabled = false;
        captureCamera.gameObject.SetActive(false);

        // ─── Tạo material dùng shader Projector, gán RenderTexture vào _ShadowTex ───
        _projectorMaterial = new Material(Shader.Find("Projector/Multiply Atlas Custom"));
        _projectorMaterial.SetTexture("_ShadowTex", _rt);

        return _projectorMaterial;
    }
}