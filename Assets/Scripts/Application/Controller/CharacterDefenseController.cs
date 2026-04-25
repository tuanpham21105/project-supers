using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDefenseController : MonoBehaviour
{
    [SerializeField] private CharacterObjectsData characterObjectsData;
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterAnimationController animationController;
    private CharacterAnimationEvents animationEvents;

    void Start()
    {
        if (characterObjectsData == null) characterObjectsData = GetComponentInParent<CharacterObjectsData>();
        if (characterStatesData == null) characterStatesData = GetComponentInParent<CharacterStatesData>();
        if (animationController == null) animationController = GetComponent<CharacterAnimationController>();

        if (characterObjectsData != null && characterObjectsData.characterMesh != null)
        {
            animationEvents = characterObjectsData.characterMesh.GetComponent<CharacterAnimationEvents>();
            if (animationEvents != null)
            {
                animationEvents.OnDeflectEnd += HandleDeflectEnd;
            }
        }
    }

    private void OnDestroy()
    {
        if (animationEvents != null)
        {
            animationEvents.OnDeflectEnd -= HandleDeflectEnd;
        }
    }

    public void Block(bool active)
    {
        // Only start blocking when not doing other upper actions OR any body actions
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag)
        {
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

    public void Deflect()
    {
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag) return;

        characterStatesData.deflectFlag = true;
        characterStatesData.upperActionFlag = true;

        if (animationController != null)
        {
            animationController.PlayUpperAnimation(CharacterUpperAnimation.deflect);
        }
    }

    private void HandleDeflectEnd()
    {
        characterStatesData.deflectFlag = false;
        characterStatesData.upperActionFlag = false;

        if (animationController != null)
        {
            animationController.EndUpperAnimation();
        }
    }
}
