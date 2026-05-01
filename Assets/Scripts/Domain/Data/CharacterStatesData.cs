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
    knock_out
}

public class CharacterStatesData : MonoBehaviour
{
    public CharacterProcessAction currentProcessAction;

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

    public event Action OnAttackInterrupt;

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

    public float moveSpeed;
    public Vector2 inputAxes;
    public Vector3 lookInput;
    public float horizontalRotation;
    public float verticalRotation;
    public Vector3 direction;

    public int currentEndurance;
}
