using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class HitSfxController : MonoBehaviour
{
    [SerializeField] private CharacterTakeDamageController characterTakeDamageController;

    [SerializeField] private AudioClip hitAudioClip;
    [SerializeField] private AudioClip deflectedAudioClip;

    void Start()
    {
        if (characterTakeDamageController != null)
        {
            characterTakeDamageController.onGetHit += handleGetHit;
        }
    }

    void OnDestroy()
    {
        if (characterTakeDamageController != null)
        {
            characterTakeDamageController.onGetHit -= handleGetHit;
        }
    }

    void handleGetHit(int damage, bool isDeflected) 
    {
        if (!isDeflected)
            GetComponent<AudioSource>().PlayOneShot(hitAudioClip);     
        else 
            GetComponent<AudioSource>().PlayOneShot(deflectedAudioClip);     
    }
}
