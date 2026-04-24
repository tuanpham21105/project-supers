using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackController : MonoBehaviour
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
    }

    public void StartNormalAttack()
    {
        if (characterStatesData.attackFlag) return;

        bool isContinuing = characterStatesData.normalAttackEndFlag;
        
        PlayNormalAttack(isContinuing);
    }

    public void StartStrikeAttack()
    {
        if (characterStatesData.attackFlag) return;

        bool isContinuing = characterStatesData.strikeAttackEndFlag;

        PlayStrikeAttack(isContinuing);
    }

    private void PlayNormalAttack(bool isContinuing)
    {
        characterStatesData.attackFlag = true;
        characterStatesData.normalAttackStartFlag = true;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = false;

        if (animationController != null)
        {
            animationController.PlayNormalAttack(isContinuing);
        }
    }

    private void PlayStrikeAttack(bool isContinuing)
    {
        characterStatesData.attackFlag = true;
        characterStatesData.strikeAttackStartFlag = true;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;

        if (animationController != null)
        {
            animationController.PlayStrikeAttack(isContinuing);
        }
    }

    private void HandleNormalAttackOngoing()
    {
        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = true;
    }

    private void HandleNormalAttackEndOngoing()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.normalAttackEndFlag = true;
    }

    private void HandleNormalAttackEnd()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.normalAttackStartFlag = false;
        characterStatesData.normalAttackOngoingFlag = false;
        characterStatesData.normalAttackEndFlag = false;

        if (animationController != null)
        {
            animationController.ResetNormalAttackCombo();
            animationController.EndUpperAnimation();
        }
    }

    private void HandleStrikeAttackOngoing()
    {
        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = true;
    }

    private void HandleStrikeAttackEndOngoing()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.strikeAttackEndFlag = true;
    }

    private void HandleStrikeAttackEnd()
    {
        characterStatesData.attackFlag = false;
        characterStatesData.strikeAttackStartFlag = false;
        characterStatesData.strikeAttackOngoingFlag = false;
        characterStatesData.strikeAttackEndFlag = false;

        if (animationController != null)
        {
            animationController.ResetStrikeAttackCombo();
            // Force the movement controller to refresh its animation state
            CharacterMovementController movementController = GetComponent<CharacterMovementController>();
            if (movementController != null)
            {
                movementController.ForceRefreshBodyAnimation();
            }
        }
    }
}
