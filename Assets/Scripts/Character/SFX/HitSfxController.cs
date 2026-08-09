using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class HitSfxController : MonoBehaviour
{
    [SerializeField] private CharacterTakeDamageController characterTakeDamageController;

    [SerializeField] private AudioClip hitAudioClip;
    [SerializeField] private AudioClip deflectedAudioClip;

    [SerializeField] private float minVolume = 0.5f;
    [SerializeField] private float maxVolume = 2f;
    [SerializeField] private int maxDamage = 1000;

    void Start()
    {
        if (characterTakeDamageController != null)
        {
            characterTakeDamageController.onGetHit += handleGetHit;
        }

        GetComponent<AudioSource>().enabled = true;
    }

    void OnDestroy()
    {
        if (characterTakeDamageController != null)
        {
            characterTakeDamageController.onGetHit -= handleGetHit;
        }
    }

    [ProButton]
    void handleGetHit(int damage, bool isDeflected) 
    {
        float volume = isDeflected ? 1f : Mathf.Lerp(minVolume, maxVolume, (float)damage / maxDamage);

        AudioClip clip = isDeflected ? deflectedAudioClip : hitAudioClip;
        GetComponent<AudioSource>().PlayOneShot(clip, volume);
    }
}
