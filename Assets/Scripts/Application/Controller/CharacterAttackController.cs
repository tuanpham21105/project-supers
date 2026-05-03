using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackController : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterObjectsData characterObjectsData;
    private CharacterObjectService characterObjectService;
    private CharacterStatesData characterStatesData;
    private CharacterStatsData characterStatsData;
    private CharacterAnimationController animationController;
    private CharacterDefenseController defenseController;
    private CharacterHitBoxesEvents characterHitBoxesEvents;
    private CharacterAnimationEvents animationEvents;

    void Start()
    {
        if (characterObjectsData == null) characterObjectsData = GetComponentInParent<CharacterObjectsData>();
        if (characterObjectService == null) characterObjectService = GetComponentInParent<CharacterObjectService>();
        if (characterStatesData == null) {
            characterStatesData = GetComponentInParent<CharacterStatesData>();
            characterStatesData.OnAttackInterrupt += HandleAttackInterrupt;
        }
        if (characterStatsData == null) characterStatsData = GetComponentInParent<CharacterStatsData>();
        if (animationController == null) animationController = GetComponent<CharacterAnimationController>();
        if (defenseController == null) defenseController = GetComponent<CharacterDefenseController>();
        if (characterHitBoxesEvents == null) characterHitBoxesEvents = characterObjectsData.characterMesh.GetComponent<CharacterHitBoxesEvents>();
        
        if (characterHitBoxesEvents != null)
        {
            characterHitBoxesEvents.OnAttackHit += HandleAttackHit;
        }

        if (characterObjectsData != null && characterObjectsData.characterMesh != null)
        {
            animationEvents = characterObjectsData.characterMesh.GetComponent<CharacterAnimationEvents>();
            if (animationEvents != null)
            {
                animationEvents.OnNormalAttackOngoing += HandleNormalAttackOngoing;
                animationEvents.OnNormalAttackEndOngoing += HandleNormalAttackEndOngoing;
                animationEvents.OnNormalAttackEnd += HandleNormalAttackEnd;
                animationEvents.OnStrikeAttackOngoing += HandleStrikeAttackOngoing;
                animationEvents.OnStrikeAttackEndOngoing += HandleStrikeAttackEndOngoing;
                animationEvents.OnStrikeAttackEnd += HandleStrikeAttackEnd;
            }
        }
    }

    private void OnDestroy()
    {
        if (animationEvents != null)
        {
            animationEvents.OnNormalAttackOngoing -= HandleNormalAttackOngoing;
            animationEvents.OnNormalAttackEndOngoing -= HandleNormalAttackEndOngoing;
            animationEvents.OnNormalAttackEnd -= HandleNormalAttackEnd;
            animationEvents.OnStrikeAttackOngoing -= HandleStrikeAttackOngoing;
            animationEvents.OnStrikeAttackEndOngoing -= HandleStrikeAttackEndOngoing;
            animationEvents.OnStrikeAttackEnd -= HandleStrikeAttackEnd;
        }

        characterStatesData.OnAttackInterrupt -= HandleAttackInterrupt;
                
        if (characterHitBoxesEvents != null)
        {
            characterHitBoxesEvents.OnAttackHit -= HandleAttackHit;
        }
    }

    public void HandleAttackInterrupt()
    {
        characterObjectsData.characterMesh.GetComponent<CharacterHitBoxesEvents>().EmitAttackInterrupt();
    }

    private void HandleAttackHit(GameObject target, AttackTypes type)
    {
        Vector3 attackDirection = (target.transform.position - gameObject.transform.position).normalized;
        int baseDamage = 0;

        switch (type)
        {
            case AttackTypes.normal_attack:
                baseDamage = characterStatsData.normalAttackDamage;
                break;
            case AttackTypes.strike_attack:
                baseDamage = characterStatsData.strikeAttackDamage;
                break;
        }

        int damage = CalculateAttackDamage(baseDamage, attackDirection);
        target.GetComponent<CharacterTakeDamageController>().GetHit(gameObject, damage, attackDirection, type);
    }

    public void ResetAttackFlags()
    {
        characterStatesData.attackFlag = false;

        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = false;

        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;

        characterStatesData.upperActionFlag = false;
        characterStatesData.bodyActionFlag = false;
    }

    private void PlayNormalAttack(bool isContinuing)
    {
        if (defenseController != null) defenseController.Block(false);

        ResetAttackFlags();
        characterStatesData.ChangeProcessAction(CharacterProcessAction.normal_attack);
        characterStatesData.attackFlag = true;
        characterStatesData.normalAttackStartFlag = true;
        characterStatesData.upperActionFlag = true;

        if (animationController != null)
        {
            animationController.PlayNormalAttack(isContinuing);
        }
    }

    private void PlayStrikeAttack(bool isContinuing)
    {
        if (defenseController != null) defenseController.Block(false);

        ResetAttackFlags();
        characterStatesData.ChangeProcessAction(CharacterProcessAction.strike_attack);
        characterStatesData.attackFlag = true;
        
        characterStatesData.strikeAttackStartFlag = true;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;

        characterStatesData.upperActionFlag = true;
        characterStatesData.bodyActionFlag = true;

        if (animationController != null)
        {
            animationController.PlayStrikeAttack(isContinuing);
        }

        //Debug.Log("Start normal attack - " + Time.time);
    }

    private void HandleNormalAttackOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.normal_attack) return;
        characterStatesData.attackFlag = true;

        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = true;
        characterStatesData.normalAttackEndFlag = false;

        //Debug.Log("Start ongoing normal attack - " + Time.time);
    }

    private void HandleNormalAttackEndOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.normal_attack) return;
        characterStatesData.attackFlag = false;

        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = true;

        characterStatesData.upperActionFlag = true;

        // Record the time this attack entered its follow-through so the next input
        // can decide whether it qualifies as a continuing combo.

        //Debug.Log("End Ongoing Normal attack - " + Time.time);
    }

    private void HandleNormalAttackEnd()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.normal_attack) return;
        if (animationController != null)
        {
            animationController.EndUpperAnimation();
        }

        characterStatesData.attackFlag = false;

        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = false;

        characterStatesData.upperActionFlag = false;

        characterStatesData.lastNormalAttackEndTime = Time.time;
        characterStatesData.ChangeProcessAction(CharacterProcessAction.none);

        //Debug.Log("End Normal attack - " + Time.time);
    }

    private void HandleStrikeAttackOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack) return;
        characterStatesData.attackFlag = true;

        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = true;
        characterStatesData.strikeAttackEndFlag = false;

        characterStatesData.upperActionFlag = true;
        characterStatesData.bodyActionFlag = true;
    }

    private void HandleStrikeAttackEndOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack) return;
        characterStatesData.attackFlag = true;

        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = true;

        characterStatesData.upperActionFlag = true;
        characterStatesData.bodyActionFlag = true;

        // Record the time this attack entered its follow-through so the next input
        // can decide whether it qualifies as a continuing combo.
    }

    private void HandleStrikeAttackEnd()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack) return;
        if (animationController != null)
        {
            // Force the movement controller to refresh its animation state
            CharacterMovementController movementController = GetComponent<CharacterMovementController>();
            if (movementController != null)
            {
                movementController.ForceRefreshBodyAnimation();
            }
        }

        characterStatesData.attackFlag = false;

        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;

        characterStatesData.upperActionFlag = false;
        characterStatesData.bodyActionFlag = false;

        characterStatesData.lastStrikeAttackEndTime = Time.time;
        characterStatesData.ChangeProcessAction(CharacterProcessAction.none);
    }

    private int CalculateAttackDamage(int baseDamage, Vector3 attackDirection)
    {
        Vector3 moveDirection = characterStatesData.currentMoveDirection;
        float sqrMoveSpeed = characterStatesData.currentSqrMoveSpeed;
        int combineDamage = baseDamage;
        
        float scale =  1 - (Vector3.Angle(moveDirection, attackDirection) / characterStatsData.maxCombineAttackAngleSize);

        combineDamage += (int)((scale > 0 ? scale : 0) * sqrMoveSpeed * characterStatsData.sqrMoveSpeedDamageThreshold);

        return combineDamage;
    }

    // [Control methods]
    public void StartNormalAttack()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.none) return;
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag) return;

        // Continuing if we are still within the combo window since the last normal attack ended.
        bool isContinuing = (Time.time - characterStatesData.lastNormalAttackEndTime) <= characterStatsData.continueAttackWindow;

        PlayNormalAttack(isContinuing);
    }

    public void StartStrikeAttack()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.none) return;
        if (characterStatesData.bodyActionFlag || characterStatesData.upperActionFlag) return;

        // Continuing if we are still within the combo window since the last strike attack ended.
        bool isContinuing = (Time.time - characterStatesData.lastStrikeAttackEndTime) <= characterStatsData.continueAttackWindow;

        PlayStrikeAttack(isContinuing);
    }

}
