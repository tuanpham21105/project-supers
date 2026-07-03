using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CapeAnimations {
    cape_float_backward,
    cape_float_forward,
    cape_float_fallback,
    cape_float_upward,
    cape_idle
}

public class CapeAnimationController : MonoBehaviour
{
    [SerializeField] private CharacterAccessoryItemData itemData;
    [SerializeField] private CharacterStatesData characterStatesData;
    private Animator animator;
    private CapeAnimations currentAnimation;

    void Start()
    {
        itemData = GetComponent<CharacterAccessoryItemData>();
        characterStatesData = itemData.character.GetComponent<CharacterStatesData>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        ControlCapeAnimation();
    }

    void ControlCapeAnimation()
    {
        if (characterStatesData == null) 
        {
            characterStatesData = itemData.character.GetComponent<CharacterStatesData>();
            return;
        }

        CapeAnimations targetAnimation;

        if (characterStatesData.fastFlyFlag)
        {
            targetAnimation = CapeAnimations.cape_idle;
        }
        else if (characterStatesData.currentPow2AllSpeed <= 100f)
        {
            targetAnimation = CapeAnimations.cape_idle;
        }
        else if (characterStatesData.verticalVelocity <= -1)
        {
            targetAnimation = CapeAnimations.cape_float_upward;
        }
        else
        {
            Vector3 forward = itemData.character.transform.forward;
            float dot = Vector3.Dot(characterStatesData.allMoveDirection.normalized, forward);

            if (dot < 0f)
            {
                targetAnimation = CapeAnimations.cape_float_forward;
            }
            else
            {
                targetAnimation = characterStatesData.currentPow2AllSpeed > 600f
                    ? CapeAnimations.cape_float_fallback
                    : CapeAnimations.cape_float_backward;
            }
        }

        if (currentAnimation != targetAnimation)
        {
            currentAnimation = targetAnimation;
            animator.CrossFadeInFixedTime(targetAnimation.ToString(), 0.5f);
        }
    }
}
