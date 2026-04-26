using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDefenseController : MonoBehaviour
{
    [SerializeField] private CharacterObjectsData characterObjectsData;
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterAnimationController animationController;
    private CharacterAnimationEvents animationEvents;

    private float lastDeflectTime;
    private float currentDeflectSpeed = 1f;
    [SerializeField] private float deflectComboWindow = 0.5f;

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
                animationEvents.OnDeflectOngoing += HandleDeflectOngoing;
                animationEvents.OnDeflectEndOngoing += HandleDeflectEndOngoing;
                animationEvents.OnDeflectEnd += HandleDeflectEnd;
            }
        }
    }

    private void OnDestroy()
    {
        if (animationEvents != null)
        {
            animationEvents.OnDeflectOngoing -= HandleDeflectOngoing;
            animationEvents.OnDeflectEndOngoing -= HandleDeflectEndOngoing;
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

        if (Time.time - lastDeflectTime < deflectComboWindow)
        {
            currentDeflectSpeed = Mathf.Min(currentDeflectSpeed + 0.75f, 2.5f);
        }
        else
        {
            currentDeflectSpeed = 1f;
        }

        characterStatesData.deflectFlag = false;
        characterStatesData.upperActionFlag = true;

        if (animationController != null)
        {
            animationController.PlayUpperAnimation(CharacterUpperAnimation.deflect_start, currentDeflectSpeed);
        }
    }

    private void HandleDeflectOngoing()
    {
        characterStatesData.deflectFlag = true;
    }

    private void HandleDeflectEndOngoing()
    {
        characterStatesData.deflectFlag = false;
    }

    private void HandleDeflectEnd()
    {
        lastDeflectTime = Time.time;
        characterStatesData.deflectFlag = false;
        characterStatesData.upperActionFlag = false;

        if (animationController != null)
        {
            animationController.EndUpperAnimation();
        }
    }
}
