using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterProcessAction
{
    none,
    normal_attack,
    strike_attack,
    deflect,
    deflected,
    knock_out,
    dead
}

public class CharacterStatesData : MonoBehaviour
{
    public void ChangeProcessAction(CharacterProcessAction nextAction)
    {
        // Turn off all flags of current action
        switch (currentProcessAction)
        {
            case CharacterProcessAction.normal_attack:
                normalAttackStartFlag = false;
                normalAttackOngoingFlag = false;
                normalAttackEndFlag = false;
                upperActionFlag = false;
                attackFlag = false;
                OnAttackInterrupt?.Invoke();
                break;
            case CharacterProcessAction.strike_attack:
                strikeAttackStartFlag = false;
                strikeAttackOngoingFlag = false;
                strikeAttackEndFlag = false;
                bodyActionFlag = false;
                upperActionFlag = false;
                attackFlag = false;
                OnAttackInterrupt?.Invoke();
                break;
            case CharacterProcessAction.deflect:
                deflectFlag = false;
                upperActionFlag = false;
                break;
            case CharacterProcessAction.deflected:
                deflectedFlag = false;
                upperActionFlag = false;
                break;
            case CharacterProcessAction.knock_out:
                knockAwayFlag = false;
                bodyActionFlag = false;
                upperActionFlag = false;
                break;
            case CharacterProcessAction.dead:
                deadFlag = false;
                bodyActionFlag = false;
                break;
        }

        if (nextAction == CharacterProcessAction.dead)
        {
            moveFlag = false;
            jumpFlag = false;
            sprintFlag = false;
            dashFlag = false;
            flyFlag = false;
            flyUpFlag = false;
            flyDownFlag = false;
            fastFlyFlag = false;
            attackFlag = false;
            blockFlag = false;
            deflectFlag = false;
            upperActionFlag = false;
            hitFlag = false;
            deflectedFlag = false;
            knockAwayFlag = false;
        }

        if (nextAction == CharacterProcessAction.knock_out)
        {
            attackFlag = false;
            strikeAttackStartFlag = false;
            strikeAttackOngoingFlag = false;
            strikeAttackEndFlag = false;
            knockAwayFlag = false;
        }

        currentProcessAction = nextAction;
    }
    
    // [Event]
    public event Action OnAttackInterrupt;

    // [Runtime]
    [Header("Runtime")]
    public CharacterProcessAction currentProcessAction;

    public bool moveFlag;
    public bool jumpFlag;
    public bool sprintFlag;
    public bool dashFlag;
    public bool dashCooldownFlag;
    public bool flyFlag;
    public bool flyUpFlag;
    public bool flyDownFlag;
    public bool fastFlyFlag;
    public bool attackFlag;
    public bool normalAttackStartFlag;
    public bool normalAttackOngoingFlag;
    public bool normalAttackEndFlag;
    public bool strikeAttackStartFlag;
    public bool strikeAttackOngoingFlag;
    public bool strikeAttackEndFlag;
    public bool knockAwayFlag;
    public bool blockFlag;
    public bool deflectFlag;
    public bool upperActionFlag;
    public bool bodyActionFlag;
    public bool hitFlag;
    public bool deflectedFlag;
    public bool deadFlag;
    public bool fallFlag;

    [Header("Character states data")]
    public int currentEndurance;
    public float controlledMoveSpeed;

    [Header("Input states data")]
    public Vector2 inputAxes;
    public Vector3 lookInput;
    public Vector3 controlledMoveDirection;

    public CharacterBodyAnimation currentBodyAnimation;

    [Header("Defense states data")]
    public float lastDeflectTime;
    public float currentDeflectSpeed = 1f;

    [Header("Attack states data")]
    public int normalAttackComboIndex = 0;
    public int strikeAttackComboIndex = 0;
    public int hitAnimationIndex = 0;
    public float lastNormalAttackEndTime = -Mathf.Infinity;
    public float lastStrikeAttackEndTime = -Mathf.Infinity;

    [Header("Movement states data")]
    public float verticalVelocity;
    public Vector3 impactForce;
    public Vector3 dashForce;
    public float dashTimer;
    public Vector3 horizontalMove;
    public bool isImpactActive;
    public Vector3 allMoveDirection;
    public float currentPow2AllSpeed;

    public bool isFront;
}
