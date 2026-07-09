using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class HitVfxManager : MonoBehaviour
{
    public static HitVfxManager instance;

    [SerializeField] private DamagePopup damgePopupPrefab;
    [SerializeField] private HitVfxParticleSystem hitVfxParticleSystem;
    [SerializeField] private HitVfxParticleSystem deflectVfxParticleSystem;
    [SerializeField] private int damagePopupPoolInitialSize = 20;
    [SerializeField] private int hitVfxPoolInitialSize = 12;
    [SerializeField] private int deflectVfxPoolInitialSize = 8;

    private readonly Queue<DamagePopup> _pool = new Queue<DamagePopup>();
    private readonly Queue<HitVfxParticleSystem> _hitVfxPool = new Queue<HitVfxParticleSystem>();
    private readonly Queue<HitVfxParticleSystem> _deflectVfxPool = new Queue<HitVfxParticleSystem>();

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
        for (int i = 0; i < damagePopupPoolInitialSize; i++)
        {
            DamagePopup popup = Instantiate(damgePopupPrefab, transform);
            popup.gameObject.SetActive(false);
            _pool.Enqueue(popup);
        }

        for (int i = 0; i < hitVfxPoolInitialSize; i++)
        {
            HitVfxParticleSystem vfx = Instantiate(hitVfxParticleSystem, transform);
            vfx.gameObject.SetActive(false);
            _hitVfxPool.Enqueue(vfx);
        }

        for (int i = 0; i < deflectVfxPoolInitialSize; i++)
        {
            HitVfxParticleSystem vfx = Instantiate(deflectVfxParticleSystem, transform);
            vfx.gameObject.SetActive(false);
            _deflectVfxPool.Enqueue(vfx);
        }
    }

    [ProButton]
    public void Show(Vector3 worldPosition, bool isAlly, float damage)
    {
        DamagePopup popup = GetFromPool();
        popup.Show(worldPosition, isAlly, damage, this);

        HitVfxParticleSystem vfx = GetHitVfxFromPool();
        vfx.Show(worldPosition, Mathf.RoundToInt(damage), ReturnHitVfxToPool);
    }

    [ProButton]
    public void ShowDeflected(Vector3 worldPosition, bool isAlly)
    {
        DamagePopup popup = GetFromPool();
        popup.ShowDeflected(worldPosition, isAlly, this);

        HitVfxParticleSystem vfx = GetDeflectVfxFromPool();
        vfx.Show(worldPosition, 0, ReturnDeflectVfxToPool);
    }

    private DamagePopup GetFromPool()
    {
        DamagePopup popup = _pool.Count > 0
            ? _pool.Dequeue()
            : Instantiate(damgePopupPrefab, transform);

        popup.gameObject.SetActive(true);
        return popup;
    }

    private HitVfxParticleSystem GetHitVfxFromPool()
    {
        HitVfxParticleSystem vfx = _hitVfxPool.Count > 0
            ? _hitVfxPool.Dequeue()
            : Instantiate(hitVfxParticleSystem, transform);

        vfx.gameObject.SetActive(true);
        return vfx;
    }

    private HitVfxParticleSystem GetDeflectVfxFromPool()
    {
        HitVfxParticleSystem vfx = _deflectVfxPool.Count > 0
            ? _deflectVfxPool.Dequeue()
            : Instantiate(deflectVfxParticleSystem, transform);

        vfx.gameObject.SetActive(true);
        return vfx;
    }

    public void ReturnToPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(false);
        _pool.Enqueue(popup);
    }

    private void ReturnHitVfxToPool(HitVfxParticleSystem vfx)
    {
        vfx.gameObject.SetActive(false);
        _hitVfxPool.Enqueue(vfx);
    }

    private void ReturnDeflectVfxToPool(HitVfxParticleSystem vfx)
    {
        vfx.gameObject.SetActive(false);
        _deflectVfxPool.Enqueue(vfx);
    }
}
