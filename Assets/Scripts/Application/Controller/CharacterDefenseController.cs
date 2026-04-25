using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDefenseController : MonoBehaviour
{
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterAnimationController animationController;

    void Start()
    {
        if (characterStatesData == null) characterStatesData = GetComponentInParent<CharacterStatesData>();
        if (animationController == null) animationController = GetComponent<CharacterAnimationController>();
    }

    public void Block(bool active)
    {
        // Only block when not attacking
        if (characterStatesData.attackFlag)
        {
            characterStatesData.blockFlag = false;
            return;
        }

        characterStatesData.blockFlag = active;

        if (animationController != null)
        {
            if (active)
            {
                animationController.PlayUpperAnimation(CharacterUpperAnimation.block);
            }
            else
            {
                // Only end upper animation if we were actually blocking
                // and not already transitioning to something else like an attack
                animationController.EndUpperAnimation();
            }
        }
    }
}
