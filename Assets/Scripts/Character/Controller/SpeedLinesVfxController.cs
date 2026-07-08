using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class SpeedLinesVfxController : MonoBehaviour
{
    private ParticleSystem speedLinesParticeSystem;

    [SerializeField] private float minEmissionRate = 0;
    [SerializeField] private float maxEmissionRate = 150;
    [SerializeField] private float maxPow2MoveSpeed = 10000;

    void Start()
    {
        speedLinesParticeSystem = GetComponent<ParticleSystem>();
    }

    [ProButton]
    public void SetMoveSpeed(float pow2MoveSpeed)
    {
        float t = Mathf.Clamp01(pow2MoveSpeed / maxPow2MoveSpeed);
        float rate = Mathf.Lerp(minEmissionRate, maxEmissionRate, t);

        var emission = speedLinesParticeSystem.emission;
        emission.rateOverTime = rate;
    }
}
