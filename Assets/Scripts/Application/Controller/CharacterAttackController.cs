using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // DEPENDENCIES
    // ─────────────────────────────────────────────
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
            characterHitBoxesEvents.OnAttackHit += HandleAttackHit;

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
            characterHitBoxesEvents.OnAttackHit -= HandleAttackHit;
    }

    // ─────────────────────────────────────────────
    // LOGIC — pure logic, no state, no presentation
    // safe to run on server
    // ─────────────────────────────────────────────

    // Checks if attack input is valid
    public bool CanStartNormalAttack()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.none) return false;
        if (characterStatesData.upperActionFlag || characterStatesData.bodyActionFlag) return false;
        return true;
    }

    public bool CanStartStrikeAttack()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.none) return false;
        if (characterStatesData.bodyActionFlag || characterStatesData.upperActionFlag) return false;
        return true;
    }

    // Checks combo window
    public bool IsNormalAttackContinuing()
    {
        return (Time.time - characterStatesData.lastNormalAttackEndTime) <= characterStatsData.continueAttackWindow;
    }

    public bool IsStrikeAttackContinuing()
    {
        return (Time.time - characterStatesData.lastStrikeAttackEndTime) <= characterStatsData.continueAttackWindow;
    }

    // Pure damage calculation — no state, no presentation
    public int CalculateAttackDamage(int baseDamage, Vector3 attackDirection)
    {
        Vector3 moveDirection = characterStatesData.currentMoveDirection;
        float sqrMoveSpeed = characterStatesData.currentSqrMoveSpeed;
        int combineDamage = baseDamage;

        float scale = 1 - (Vector3.Angle(moveDirection, attackDirection) / characterStatsData.maxCombineAttackAngleSize);
        combineDamage += (int)((scale > 0 ? scale : 0) * sqrMoveSpeed * characterStatsData.sqrMoveSpeedDamageThreshold);

        return combineDamage;
    }

    // ─────────────────────────────────────────────
    // STATE — only modify state data
    // safe to run on server and sync to clients
    // ─────────────────────────────────────────────

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

    public void SetStateNormalAttackStart()
    {
        ResetAttackFlags();
        characterStatesData.ChangeProcessAction(CharacterProcessAction.normal_attack);
        characterStatesData.attackFlag = true;
        characterStatesData.normalAttackStartFlag = true;
        characterStatesData.upperActionFlag = true;
    }

    public void SetStateNormalAttackOngoing()
    {
        characterStatesData.attackFlag = true;
        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = true;
        characterStatesData.normalAttackEndFlag = false;
    }

    public void SetStateNormalAttackEndOngoing()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = true;
        characterStatesData.upperActionFlag = true;
    }

    public void SetStateNormalAttackEnd()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = false;
        characterStatesData.upperActionFlag = false;
        characterStatesData.lastNormalAttackEndTime = Time.time;
        characterStatesData.ChangeProcessAction(CharacterProcessAction.none);
    }

    public void SetStateStrikeAttackStart()
    {
        ResetAttackFlags();
        characterStatesData.ChangeProcessAction(CharacterProcessAction.strike_attack);
        characterStatesData.attackFlag = true;
        characterStatesData.strikeAttackStartFlag = true;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;
        characterStatesData.upperActionFlag = true;
        characterStatesData.bodyActionFlag = true;
    }

    public void SetStateStrikeAttackOngoing()
    {
        characterStatesData.attackFlag = true;
        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = true;
        characterStatesData.strikeAttackEndFlag = false;
        characterStatesData.upperActionFlag = true;
        characterStatesData.bodyActionFlag = true;
    }

    public void SetStateStrikeAttackEndOngoing()
    {
        characterStatesData.attackFlag = true;
        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = true;
        characterStatesData.upperActionFlag = true;
        characterStatesData.bodyActionFlag = true;
    }

    public void SetStateStrikeAttackEnd()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;
        characterStatesData.upperActionFlag = false;
        characterStatesData.bodyActionFlag = false;
        characterStatesData.lastStrikeAttackEndTime = Time.time;
        characterStatesData.ChangeProcessAction(CharacterProcessAction.none);
    }

    // ─────────────────────────────────────────────
    // PRESENTATION — animation, sound, visual only
    // run on client only, never sync
    // ─────────────────────────────────────────────

    private void PlayNormalAttackPresentation(bool isContinuing)
    {
        if (defenseController != null) defenseController.Block(false);
        if (animationController != null) animationController.PlayNormalAttack(isContinuing);
    }

    private void PlayStrikeAttackPresentation(bool isContinuing)
    {
        if (defenseController != null) defenseController.Block(false);
        if (animationController != null) animationController.PlayStrikeAttack(isContinuing);
    }

    private void EndNormalAttackPresentation()
    {
        if (animationController != null) animationController.EndUpperAnimation();
    }

    private void EndStrikeAttackPresentation()
    {
        CharacterMovementController movementController = GetComponent<CharacterMovementController>();
        if (movementController != null) movementController.ForceRefreshBodyAnimation();
    }

    public void HandleAttackInterrupt()
    {
        // presentation only — emit interrupt visual/animation
        characterObjectsData.characterMesh.GetComponent<CharacterHitBoxesEvents>().EmitAttackInterrupt();
    }

    // ─────────────────────────────────────────────
    // CONTROLLERS — entry points, combines logic + state + presentation
    // on server: call logic + state only
    // on client: call all three
    // ─────────────────────────────────────────────

    public void StartNormalAttack()
    {
        if (!CanStartNormalAttack()) return;            // logic
        bool isContinuing = IsNormalAttackContinuing(); // logic
        SetStateNormalAttackStart();                     // state
        PlayNormalAttackPresentation(isContinuing);      // presentation
    }

    public void StartStrikeAttack()
    {
        if (!CanStartStrikeAttack()) return;             // logic
        bool isContinuing = IsStrikeAttackContinuing();  // logic
        SetStateStrikeAttackStart();                     // state
        PlayStrikeAttackPresentation(isContinuing);      // presentation
    }

    private void HandleAttackHit(GameObject target, AttackTypes type)
    {
        // logic
        Vector3 attackDirection = (target.transform.position - gameObject.transform.position).normalized;
        int baseDamage = type == AttackTypes.normal_attack
            ? characterStatsData.normalAttackDamage
            : characterStatsData.strikeAttackDamage;
        int damage = CalculateAttackDamage(baseDamage, attackDirection); // logic

        // state + presentation on target
        target.GetComponent<CharacterTakeDamageController>().GetHit(gameObject, damage, attackDirection, type);
    }

    // ─────────────────────────────────────────────
    // ANIMATION EVENT HANDLERS
    // each splits into state + presentation
    // ─────────────────────────────────────────────

    private void HandleNormalAttackOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.normal_attack) return;
        SetStateNormalAttackOngoing(); // state
        // no presentation change here
    }

    private void HandleNormalAttackEndOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.normal_attack) return;
        SetStateNormalAttackEndOngoing(); // state
        // no presentation change here
    }

    private void HandleNormalAttackEnd()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.normal_attack) return;
        SetStateNormalAttackEnd();       // state
        EndNormalAttackPresentation();   // presentation
    }

    private void HandleStrikeAttackOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack) return;
        SetStateStrikeAttackOngoing(); // state
        // no presentation change here
    }

    private void HandleStrikeAttackEndOngoing()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack) return;
        SetStateStrikeAttackEndOngoing(); // state
        // no presentation change here
    }

    private void HandleStrikeAttackEnd()
    {
        if (characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack) return;
        SetStateStrikeAttackEnd();      // state
        EndStrikeAttackPresentation();  // presentation
    }
}
