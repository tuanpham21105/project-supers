using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSfxController : MonoBehaviour
{
    [SerializeField] private CharacterAnimationService characterAnimationService;

    [SerializeField] private AudioClip walkSfx;
    [SerializeField] private AudioClip runSfx;
    [SerializeField] private AudioClip fallImpactSfx;

    void Start()
    {
        if (characterAnimationService != null)
        {
            characterAnimationService.onPlayAnimation += handleAnimationPlay;
        }
    }

    void OnDestroy()
    {
        if (characterAnimationService != null)
        {
            characterAnimationService.onPlayAnimation -= handleAnimationPlay;
        }
    }

    void handleAnimationPlay(string part, string animation)
    {
        if (part.Equals("body"))
        {
            if (
                animation.Equals(CharacterBodyAnimation.walking_forward.ToString()) ||
                animation.Equals(CharacterBodyAnimation.walking_backward.ToString()) ||
                animation.Equals(CharacterBodyAnimation.walking_right.ToString()) ||
                animation.Equals(CharacterBodyAnimation.walking_left.ToString())
            )
            {
                GetComponent<AudioSource>().clip = walkSfx;
                GetComponent<AudioSource>().Play();
            }
            else if (
                animation.Equals(CharacterBodyAnimation.sprint_forward.ToString()) ||
                animation.Equals(CharacterBodyAnimation.sprint_backward.ToString()) ||
                animation.Equals(CharacterBodyAnimation.sprint_right.ToString()) ||
                animation.Equals(CharacterBodyAnimation.sprint_left.ToString())
            )
            {
                GetComponent<AudioSource>().clip = runSfx;
                GetComponent<AudioSource>().Play();
            }
            else
            {
                GetComponent<AudioSource>().Stop();
            }
        }
    }
}