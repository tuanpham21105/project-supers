using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Giữ chuột trái và kéo sang trái/phải để xoay object quanh trục Y.
/// Tối ưu: dùng MonoBehaviour.enabled để tắt/bật — Unity tự bỏ qua Update()
/// hoàn toàn khi disabled, không tốn chi phí gì khi tắt.
/// </summary>
public class DragRotateObject : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Object cần xoay. Để trống sẽ tự dùng chính GameObject này.")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 0.3f;
    [SerializeField] private bool invertDirection = false;

    [Tooltip("Bỏ qua thao tác kéo nếu chuột đang ở trên UI (nút bấm, panel...)")]
    [SerializeField] private bool ignoreWhenOverUI = true;

    private bool _isDragging;
    private Vector3 _lastMousePosition;

    void Awake()
    {
        if (target == null) target = transform;
    }

    void OnDisable()
    {
        // Reset trạng thái kéo khi bị tắt giữa chừng, tránh giật khi bật lại
        _isDragging = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ignoreWhenOverUI && EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            return;
        }

        if (!_isDragging) return;

        Vector3 currentMousePosition = Input.mousePosition;
        float deltaX = currentMousePosition.x - _lastMousePosition.x;

        if (deltaX != 0f)
        {
            float rotationAmount = deltaX * rotationSpeed * (invertDirection ? -1f : 1f);
            target.Rotate(Vector3.up, rotationAmount, Space.World);
        }

        _lastMousePosition = currentMousePosition;
    }

    // ─────────────────────────────────────────────
    // Public API — tắt/bật khi cần
    // ─────────────────────────────────────────────

    /// <summary>Bật/tắt tính năng kéo xoay. Khi tắt, Update() không chạy — 0 chi phí.</summary>
    public void SetEnabled(bool value)
    {
        enabled = value;
    }

    public void Toggle()
    {
        enabled = !enabled;
    }

    public bool IsEnabled() => enabled;
}
