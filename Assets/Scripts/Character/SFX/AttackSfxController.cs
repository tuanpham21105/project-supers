using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AttackSfxController : MonoBehaviour
{
    [SerializeField] private CharacterAnimationEvents characterAnimationEvents;

    [SerializeField] private AudioClip deflectSfx;
    [SerializeField] private AudioClip normalAttackSfx;
    [SerializeField] private AudioClip strikeAttackSfx;

    void Start()
    {
        if (characterAnimationEvents != null)
        {
            characterAnimationEvents.OnNormalAttackOngoing += handleAttackOngoing(true);
            characterAnimationEvents.OnStrikeAttackOngoing += handleAttackOngoing(false);
            characterAnimationEvents.OnDeflectOngoing += handleDeflectOngoing;
        }

        GetComponent<AudioSource>().enabled = true;
    }

    void OnDestroy()
    {
        if (characterAnimationEvents != null)
        {
            characterAnimationEvents.OnNormalAttackOngoing -= handleAttackOngoing(true);
            characterAnimationEvents.OnStrikeAttackOngoing -= handleAttackOngoing(false);
            characterAnimationEvents.OnDeflectOngoing -= handleDeflectOngoing;
        }
    }

    Action handleAttackOngoing(bool isNormalAttack)
    {
        return () =>
        {
            Play(isNormalAttack);
        };
    }


    void handleDeflectOngoing()
    {
        GetComponent<AudioSource>().PlayOneShot(deflectSfx);
    }

    public void Play(bool isNormalAttack)
    {
        GetComponent<AudioSource>().PlayOneShot(isNormalAttack ? normalAttackSfx : strikeAttackSfx);
    }
}
