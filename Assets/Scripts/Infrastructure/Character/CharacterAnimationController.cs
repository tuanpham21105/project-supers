using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterLowerAnimation
{
    ground_idle,
    walking_forward,
    walking_backward,
    walking_right,
    walking_left,
    sprint_forward,
    sprint_backward,
    sprint_right,
    sprint_left,
    fly_idle,
    fly_forward,
    fly_backward,
    fly_right,
    fly_left,
    fast_fly,
    jump,
    fall
}

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayLowerAnimation(CharacterLowerAnimation animation)
    {
        if (animator != null)
        {
            animator.CrossFade(animation.ToString(), 0.3f);
        }
    }
}
