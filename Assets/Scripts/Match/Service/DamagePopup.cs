using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Prefab này dùng TextMeshPro (3D world text, KHÔNG phải TextMeshProUGUI).
/// Đặt component này trên GameObject có sẵn TextMeshPro component.
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class DamagePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro tmp;

    [Header("Colors")]
    [SerializeField] private Color enemyColor = new Color(1f, 0.15f, 0.15f);   // đỏ — đánh trúng địch
    [SerializeField] private Color allyColor  = new Color(1f, 0.85f, 0.1f);    // vàng — ta bị đánh

    [Header("Distance theo Damage")]
    [SerializeField] private float minDistance = 0.4f;
    [SerializeField] private float maxDistance = 2.2f;

    [Header("Font Size theo Damage")]
    [SerializeField] private float minFontSize = 3.5f;
    [SerializeField] private float maxFontSize = 9f;

    [Tooltip("Damage đạt tới giá trị này sẽ dùng distance/fontSize tối đa")]
    [SerializeField] private float damageForMaxScale = 200f;

    [Tooltip("Chữ dành cho phe ta luôn nhỏ hơn địch theo tỉ lệ này, không phụ thuộc damage")]
    [SerializeField] private float allySizeMultiplier = 0.6f;

    [Header("Random Direction")]
    [Tooltip("Độ lệch ngang ngẫu nhiên so với hướng thẳng lên/xuống (0 = luôn thẳng, 1 = ngẫu nhiên hoàn toàn)")]
    [SerializeField, Range(0f, 2f)] private float horizontalRandomness = 0.5f;

    [Header("Timing")]
    [SerializeField] private float duration = 1f;

    [Header("Animation Curves")]
    [Tooltip("Tiến trình di chuyển theo thời gian chuẩn hóa (X: 0-1 thời gian, Y: 0-1 quãng đường đã đi)")]
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Alpha theo thời gian chuẩn hóa — tự thiết kế fade in đầu + fade out cuối trong 1 curve")]
    [SerializeField] private AnimationCurve fadeCurve = new AnimationCurve(
        new Keyframe(0f,   0f),
        new Keyframe(0.15f, 1f),
        new Keyframe(0.75f, 1f),
        new Keyframe(1f,   0f)
    );

    [Tooltip("Scale nhân thêm theo thời gian (VD: pop-in hiệu ứng nảy nhẹ lúc xuất hiện)")]
    [SerializeField] private AnimationCurve scaleCurve = new AnimationCurve(
        new Keyframe(0f,   0.6f),
        new Keyframe(0.15f, 1.1f),
        new Keyframe(0.3f,  1f),
        new Keyframe(1f,   1f)
    );

    private Camera _cam;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _baseFontSize;
    private DamagePopupManager _manager;
    private Coroutine _routine;

    private static Material _sharedAlwaysOnTopMaterial;

    [SerializeField] private bool renderAlwaysOnTop = true;

    void Awake()
    {
        if (tmp == null) tmp = GetComponent<TextMeshPro>();
        _cam = Camera.main;

        if (renderAlwaysOnTop)
        {
            if (_sharedAlwaysOnTopMaterial == null)
            {
                _sharedAlwaysOnTopMaterial = new Material(tmp.fontMaterial);
                _sharedAlwaysOnTopMaterial.SetFloat("_ZTest", 
                    (float)UnityEngine.Rendering.CompareFunction.Always);
            }

            tmp.fontSharedMaterial = _sharedAlwaysOnTopMaterial; // ✅ dùng chung
        }
    }

    public void Show(Vector3 worldPosition, bool isAlly, float damage, DamagePopupManager manager)
    {
        _manager = manager;
        transform.position = worldPosition;

        // ─── Text & Color ───
        tmp.text  = BigNumberStringify.decorate(Mathf.RoundToInt(damage));
        tmp.color = isAlly ? allyColor : enemyColor;

        // ─── Scale theo damage (địch to hơn ta luôn) ───
        float damageT = Mathf.Clamp01(damage / damageForMaxScale);
        float fontSize = Mathf.Lerp(minFontSize, maxFontSize, damageT);
        if (isAlly) fontSize *= allySizeMultiplier;
        _baseFontSize = fontSize;
        tmp.fontSize = fontSize;

        // ─── Hướng bay: địch lên / ta xuống + lệch ngang random ───
        float distance = Mathf.Lerp(minDistance, maxDistance, damageT);

        Vector3 verticalDir = isAlly ? Vector3.down : Vector3.up;
        Vector2 randomHorizontal = Random.insideUnitCircle.normalized;
        Vector3 horizontalDir = new Vector3(randomHorizontal.x, 0f, randomHorizontal.y);

        Vector3 direction = (verticalDir + horizontalDir * horizontalRandomness).normalized;

        _startPos = worldPosition;
        _endPos   = worldPosition + direction * distance;

        // ─── Billboard ngay từ đầu để tránh giật hình frame đầu ───
        FaceCamera();

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(AnimateRoutine());
    }

    public void ShowDeflected(Vector3 worldPosition, bool isAlly,  DamagePopupManager manager)
    {
        _manager = manager;
        transform.position = worldPosition;

        tmp.text  = "DEFLECT";
        tmp.color = isAlly ? allyColor : enemyColor;

        float fontSize = maxFontSize;
        if (isAlly) fontSize *= allySizeMultiplier;
        _baseFontSize = fontSize;
        tmp.fontSize = fontSize;

        float distance = maxDistance;

        Vector3 verticalDir = isAlly ? Vector3.down : Vector3.up;
        Vector2 randomHorizontal = Random.insideUnitCircle.normalized;
        Vector3 horizontalDir = new Vector3(randomHorizontal.x, 0f, randomHorizontal.y);

        Vector3 direction = (verticalDir + horizontalDir * horizontalRandomness).normalized;

        _startPos = worldPosition;
        _endPos   = worldPosition + direction * distance;

        FaceCamera();

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);

            // Di chuyển theo moveCurve
            float moveT = moveCurve.Evaluate(normalizedTime);
            transform.position = Vector3.Lerp(_startPos, _endPos, moveT);

            // Fade theo fadeCurve
            float alpha = fadeCurve.Evaluate(normalizedTime);
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;

            // Scale pop theo scaleCurve
            float scaleT = scaleCurve.Evaluate(normalizedTime);
            tmp.fontSize = _baseFontSize * scaleT;

            // Luôn quay mặt về camera
            FaceCamera();

            yield return null;
        }

        _manager.ReturnToPool(this);
    }

    private void FaceCamera()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null) return;
        }

        transform.rotation = _cam.transform.rotation;
    }
}
