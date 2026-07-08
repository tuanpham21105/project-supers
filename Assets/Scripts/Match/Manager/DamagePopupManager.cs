using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

/// <summary>
/// Quản lý pool DamagePopup và cung cấp hàm Show() để gọi từ bất kỳ đâu.
/// Đặt component này trên 1 GameObject trong scene, gán prefab DamagePopup.
/// </summary>
public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager instance;

    [SerializeField] private DamagePopup prefab;
    [SerializeField] private int poolInitialSize = 20;

    private readonly Queue<DamagePopup> _pool = new Queue<DamagePopup>();

    void Awake()
    {
        instance = this;
        Prewarm();
    }

    void OnDestroy()
    {
        instance = null;   
    }

    private void Prewarm()
    {
        for (int i = 0; i < poolInitialSize; i++)
        {
            DamagePopup popup = Instantiate(prefab, transform);
            popup.gameObject.SetActive(false);
            _pool.Enqueue(popup);
        }
    }

    /// <summary>
    /// Hiển thị damage popup tại vị trí world.
    /// </summary>
    /// <param name="worldPosition">Vị trí xuất hiện</param>
    /// <param name="isAlly">true = phe ta (vàng, bay xuống, chữ nhỏ hơn), false = địch (đỏ, bay lên, chữ to hơn)</param>
    /// <param name="damage">Giá trị damage — ảnh hưởng độ xa bay và kích cỡ chữ</param>
    [ProButton]
    public void Show(Vector3 worldPosition, bool isAlly, float damage)
    {
        DamagePopup popup = GetFromPool();
        popup.Show(worldPosition, isAlly, damage, this);
    }

    private DamagePopup GetFromPool()
    {
        DamagePopup popup = _pool.Count > 0
            ? _pool.Dequeue()
            : Instantiate(prefab, transform);

        popup.gameObject.SetActive(true);
        return popup;
    }

    public void ReturnToPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(false);
        _pool.Enqueue(popup);
    }
}
