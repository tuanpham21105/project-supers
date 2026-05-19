using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDefenseController : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterObjectsData characterObjectsData;
    private CharacterStatesData characterStatesData;
    private CharacterAnimationController animationController;
    private CharacterAttackController characterAttackController;
    private CharacterAnimationEvents animationEvents;
    private CharacterStatsData characterStatsData;

    void Start()
    {
        if (characterObjectsData == null) characterObjectsData = GetComponent<CharacterObjectsData>();
        if (characterStatesData == null) characterStatesData = GetComponent<CharacterStatesData>();
        if (animationController == null) animationController = GetComponent<CharacterAnimationController>();
        if (characterAttackController == null) characterAttackController = GetComponent<CharacterAttackController>();
        if (characterStatsData == null) characterStatsData = GetComponent<CharacterStatsData>();

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

    private void HandleDeflectOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.deflect) return;
        characterStatesData.deflectFlag = true;

        characterStatesData.upperActionFlag = true;

        //Debug.Log("Start Ongoing deflect - " + Time.time);
    }

    private void HandleDeflectEndOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.deflect) return;
        characterStatesData.deflectFlag = false;

        characterStatesData.upperActionFlag = true;

        //Debug.Log("End Ongoing deflect - " + Time.time);
    }

    public void HandleDeflectEnd()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.deflect) return;
        if (animationController != null)
        {
            animationController.EndUpperAnimation();
        }

        characterStatesData.lastDeflectTime = Time.time;

        characterStatesData.deflectFlag = false;

        characterStatesData.upperActionFlag = false;
        characterStatesData.ChangeProcessAction(CharacterProcessAction.none);

        //Debug.Log("End deflect - " + Time.time);
    }

    // [Control methods]
    public void Block(bool active)
    {
        // Only start blocking when not doing other upper actions OR any body actions
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag)
        {
            characterStatesData.blockFlag = false;
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
        if (characterStatesData.currentProcessAction != CharacterProcessAction.none) return;
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag) return;

        if (Time.time - characterStatesData.lastDeflectTime < characterStatsData.deflectComboWindow)
        {
            characterStatesData.currentDeflectSpeed = Mathf.Min(characterStatesData.currentDeflectSpeed + 0f, 2.5f);
        }
        else
        {
            characterStatesData.currentDeflectSpeed = 1f;
        }

        ResetDefenseFlags();
        characterStatesData.ChangeProcessAction(CharacterProcessAction.deflect);
        characterStatesData.upperActionFlag = true;

        if (animationController != null)
        {
            animationController.PlayUpperAnimation(CharacterUpperAnimation.deflect);
        }

        //Debug.Log("Start deflect - " + Time.time);
    }

}
