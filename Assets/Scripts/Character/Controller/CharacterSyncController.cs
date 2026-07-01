using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using Unity.Collections;
using UnityEngine;

[Serializable]
public enum CharacterAnimationTypes
{
    upper,
    body,
    addition
}

public class CharacterSyncController : MonoBehaviour
{
    [Header("Dependencies")]
    private CharacterAnimationService characterAnimationController;

    void Start()
    {
        characterAnimationController = GetComponent<CharacterAnimationService>();
    }

    [ProButton]
    public void PlayAnimation(CharacterAnimationTypes animationType, String animationName)
    {
        switch (animationType)
        {
            case CharacterAnimationTypes.body:
                if (string.IsNullOrEmpty(animationName))
                {
                    animationName = "ground_idle";
                }
                if (Enum.TryParse(animationName, true, out CharacterBodyAnimation bodyAnim))
                {
                    characterAnimationController.PlayBodyAnimation(bodyAnim);
                }
                break;

            case CharacterAnimationTypes.upper:
                if (string.IsNullOrEmpty(animationName))
                {
                    animationName = "none";
                }
                
                if (Enum.TryParse(animationName, true, out CharacterUpperAnimation upperAnim))
                {
                if (upperAnim == CharacterUpperAnimation.none)
                    characterAnimationController.EndUpperAnimation();
                else
                    characterAnimationController.PlayUpperAnimation(upperAnim);
                }
                break;

            case CharacterAnimationTypes.addition:
                if (string.IsNullOrEmpty(animationName))
                {
                    animationName = "none";
                }
                if (Enum.TryParse(animationName, true, out AdditionalAnimation addAnim))
                {
                    characterAnimationController.PlayAdditionalAnimation(addAnim);
                }
                break;
        }
    }
   
    public void ApplyTransform(Vector3 position, Vector3 forward)
    {
        transform.position = position;
        transform.forward = forward;
    }

    public void ApplyPhysicsCollider(float radius, float height)
    {
        GetComponent<CharacterController>().radius = radius;
        GetComponent<CharacterController>().height = height;
    }

    public void ApplyStates(CharacterStatesDto statesDto)
    {
        CharacterStatesData statesData = GetComponent<CharacterStatesData>();

        if (Enum.TryParse(statesDto.currentProcessAction, out CharacterProcessAction processAction))
        {
            statesData.currentProcessAction = processAction;
        }

        statesData.moveFlag = statesDto.moveFlag;
        statesData.jumpFlag = statesDto.jumpFlag;
        statesData.sprintFlag = statesDto.sprintFlag;
        statesData.dashFlag = statesDto.dashFlag;
        statesData.dashCooldownFlag = statesDto.dashCooldownFlag;
        statesData.flyFlag = statesDto.flyFlag;
        statesData.flyUpFlag = statesDto.flyUpFlag;
        statesData.flyDownFlag = statesDto.flyDownFlag;
        statesData.fastFlyFlag = statesDto.fastFlyFlag;
        statesData.attackFlag = statesDto.attackFlag;
        statesData.normalAttackStartFlag = statesDto.normalAttackStartFlag;
        statesData.normalAttackOngoingFlag = statesDto.normalAttackOngoingFlag;
        statesData.normalAttackEndFlag = statesDto.normalAttackEndFlag;
        statesData.strikeAttackStartFlag = statesDto.strikeAttackStartFlag;
        statesData.strikeAttackOngoingFlag = statesDto.strikeAttackOngoingFlag;
        statesData.strikeAttackEndFlag = statesDto.strikeAttackEndFlag;
        statesData.knockAwayFlag = statesDto.knockAwayFlag;
        statesData.blockFlag = statesDto.blockFlag;
        statesData.deflectFlag = statesDto.deflectFlag;
        statesData.upperActionFlag = statesDto.upperActionFlag;
        statesData.bodyActionFlag = statesDto.bodyActionFlag;
        statesData.hitFlag = statesDto.hitFlag;
        statesData.deflectedFlag = statesDto.deflectedFlag;
        statesData.deadFlag = statesDto.deadFlag;
        statesData.fallFlag = statesDto.fallFlag;

        statesData.currentEndurance = statesDto.currentEndurance;
        statesData.moveSpeed = statesDto.moveSpeed;

        statesData.inputAxes = statesDto.inputAxes.ToVector2();
        statesData.lookInput = statesDto.lookInput.ToVector3();
        statesData.direction = statesDto.direction.ToVector3();

        if (Enum.TryParse(statesDto.currentBodyAnimation, out CharacterBodyAnimation bodyAnimation))
        {
            statesData.currentBodyAnimation = bodyAnimation;
        }

        statesData.lastNormalAttackEndTime = statesDto.lastNormalAttackEndTime;
        statesData.lastStrikeAttackEndTime = statesDto.lastStrikeAttackEndTime;

        statesData.lastDeflectTime = statesDto.lastDeflectTime;
        statesData.currentDeflectSpeed = statesDto.currentDeflectSpeed;

        statesData.normalAttackComboIndex = statesDto.normalAttackComboIndex;
        statesData.strikeAttackComboIndex = statesDto.strikeAttackComboIndex;
        statesData.hitAnimationIndex = statesDto.hitAnimationIndex;

        statesData.verticalVelocity = statesDto.verticalVelocity;
        statesData.impactForce = statesDto.impactForce.ToVector3();
        statesData.dashForce = statesDto.dashForce.ToVector3();
        statesData.dashTimer = statesDto.dashTimer;
        statesData.horizontalMove = statesDto.horizontalMove.ToVector3();
        statesData.isImpactActive = statesDto.isImpactActive;
        statesData.currentMoveDirection = statesDto.currentMoveDirection.ToVector3();
        statesData.currentSqrMoveSpeed = statesDto.currentSqrMoveSpeed;

        statesData.isFront = statesDto.isFront;
    }
}
