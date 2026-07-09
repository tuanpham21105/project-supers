using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySfxController : MonoBehaviour
{
    [SerializeField] private CharacterStatesData characterStatesData;

    [SerializeField] private float minVolume = 0;
    [SerializeField] private float maxVolume = 1;
    [SerializeField] private float maxPow2MoveSpeed = 5000;
    [SerializeField] private float smoothTime = 0.1f;

    private AudioSource audioSource;
    private float currentVolume;
    private float refVelocity;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        float t = Mathf.Clamp01(characterStatesData.currentPow2AllSpeed / maxPow2MoveSpeed);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, t);
        currentVolume = Mathf.SmoothDamp(currentVolume, targetVolume, ref refVelocity, smoothTime);
        audioSource.volume = currentVolume;
    }
}
