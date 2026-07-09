using System;
using UnityEngine;

public class HitVfxParticleSystem : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Action<HitVfxParticleSystem> _returnToPool;

    [Header("Size range")]
    [SerializeField] private float minSize = 1;
    [SerializeField] private float maxSize = 5;
    [SerializeField] public int maxDamage = 1000;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Show(Vector3 worldPosition, int damage, Action<HitVfxParticleSystem> returnCallback)
    {
        _returnToPool = returnCallback;

        transform.position = worldPosition;

        float t = Mathf.Clamp01(damage / (float)maxDamage);
        float size = Mathf.Lerp(minSize, maxSize, t);
        transform.localScale = Vector3.one * size;

        if (_particleSystem != null)
            _particleSystem.Play();
    }

    void OnParticleSystemStopped()
    {
        _returnToPool?.Invoke(this);
    }
}
