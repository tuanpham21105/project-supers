using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDefenseController : MonoBehaviour
{
    [SerializeField] private CharacterObjectsData characterObjectsData;
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterAnimationController animationController;
    [SerializeField] private CharacterAttackController characterAttackController;
    private CharacterAnimationEvents animationEvents;

    private float lastDeflectTime;
    private float currentDeflectSpeed = 1f;
    [SerializeField] private float deflectComboWindow = 0.5f;

    void Start()
    {
        if (characterObjectsData == null) characterObjectsData = GetComponentInParent<CharacterObjectsData>();
        if (characterStatesData == null) characterStatesData = GetComponentInParent<CharacterStatesData>();
        if (animationController == null) animationController = GetComponent<CharacterAnimationController>();
        if (characterAttackController == null) characterAttackController = GetComponentInParent<CharacterAttackController>();

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

    public void ResetDefenseFlags()
    {
        characterStatesData.blockFlag = false;
        characterStatesData.deflectFlag = false;
        characterStatesData.upperActionFlag = false;
    }

    public void Block(bool active)
    {
        // Only start blocking when not doing other upper actions OR any body actions
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag)
        {
            return;
        }

        ResetDefenseFlags();
        characterStatesData.blockFlag = active;

        if (animationController != null)
        {
            if (active)
            {
                // characterStatesData.upperActionFlag = true;
                animationController.PlayUpperAnimation(CharacterUpperAnimation.block);
            }
            else
            {
                animationController.EndUpperAnimation();
            }
        }
    }

    public void Deflect()
    {
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag) return;

        if (Time.time - lastDeflectTime < deflectComboWindow)
        {
            currentDeflectSpeed = Mathf.Min(currentDeflectSpeed + 0f, 2.5f);
        }
        else
        {
            currentDeflectSpeed = 1f;
        }

        ResetDefenseFlags();
        characterStatesData.upperActionFlag = true;

        if (animationController != null)
        {
            animationController.PlayUpperAnimation(CharacterUpperAnimation.deflect, currentDeflectSpeed);
        }

        //Debug.Log("Start deflect - " + Time.time);
    }

    private void HandleDeflectOngoing()
    {
        characterStatesData.deflectFlag = true;

        characterStatesData.upperActionFlag = true;

        //Debug.Log("Start Ongoing deflect - " + Time.time);
    }

    private void HandleDeflectEndOngoing()
    {
        characterStatesData.deflectFlag = false;

        characterStatesData.upperActionFlag = true;

        //Debug.Log("End Ongoing deflect - " + Time.time);
    }

    public void HandleDeflectEnd()
    {
        if (animationController != null)
        {
            animationController.EndUpperAnimation();
        }

        lastDeflectTime = Time.time;

        characterStatesData.deflectFlag = false;

        characterStatesData.upperActionFlag = false;

        //Debug.Log("End deflect - " + Time.time);
    }
}
