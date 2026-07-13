using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionController : MonoBehaviour
{
    [Header("Dependencies")]
    private CharacterMovementController characterMovementController;
    private CharacterAttackController characterAttackController;
    private CharacterDefenseController characterDefenseController;

    [Header("Runtime")]

    [SerializeField] private bool sprintInput;
    [SerializeField] private bool dashInput;
    [SerializeField] private bool toggleFlyInput;
    [SerializeField] private bool jumpInput;
    [SerializeField] private bool flyUpInput;
    [SerializeField] private bool flyDownInput;
    [SerializeField] private bool normalAttackInput;
    [SerializeField] private bool strikeAttackInput;
    [SerializeField] private bool blockInput;
    [SerializeField] private bool deflectInput;
    [SerializeField] private bool forwardInput;
    [SerializeField] private bool backwardInput;
    [SerializeField] private bool leftInput;
    [SerializeField] private bool rightInput;
    
    public event Action onFlyingInterrupted;

    private event Action onUpdateActions;

    void Start()
    {
        characterMovementController = GetComponent<CharacterMovementController>();
        characterAttackController = GetComponent<CharacterAttackController>();
        characterDefenseController = GetComponent<CharacterDefenseController>();

        characterMovementController.endFlying += HandleFlyingInterrupted;

        if (MatchData.hostPlayer == PlayerData.instance.username) 
            onUpdateActions += UpdateActions;
    }

    void Update()
    {
        onUpdateActions?.Invoke();
    }

    void UpdateActions()
    {
        MoveDirection();
        Sprint();
        Dash();
        ToggleFly();
        Jump();
        FlyUp();
        FlyDown();
        NormalAttack();
        StrikeAttack();
        Block();
        Deflect();
    }

    void OnDestroy()
    {
        characterMovementController.endFlying -= HandleFlyingInterrupted;

        if (MatchData.hostPlayer != PlayerData.instance.username) 
            onUpdateActions -= UpdateActions;
    }

    void HandleFlyingInterrupted()
    {
        onFlyingInterrupted?.Invoke();
        toggleFlyInput = false;
    }

    private void MoveDirection()
    {
        float x = 0;
        float y = 0;

        if (forwardInput) y += 1;
        if (backwardInput) y -= 1;
        if (leftInput) x -= 1;
        if (rightInput) x += 1;

        Vector2 currentMoveInput = new Vector2(x, y).normalized;

        characterMovementController.Move(currentMoveInput);
    }

    public void Sprint()
    {
        characterMovementController.SetSprint(sprintInput);
    }

    public void Dash()
    {
        if (dashInput)
            characterMovementController.Dash();
    }

    public void ToggleFly()
    {
        characterMovementController.SetFly(toggleFlyInput);
    }

    public void Jump()
    {
        if (jumpInput)
            characterMovementController.Jump();
    }

    public void FlyUp()
    {
        characterMovementController.SetFlyUp(flyUpInput);
    }

    public void FlyDown()
    {
        characterMovementController.SetFlyDown(flyDownInput);
    }

    public void NormalAttack()
    {
        if (normalAttackInput)
            characterAttackController.StartNormalAttack();
    }

    public void StrikeAttack()
    {
        if (strikeAttackInput)
            characterAttackController.StartStrikeAttack();
    }

    public void Block()
    {
        characterDefenseController.Block(blockInput);
    }

    public void Deflect()
    {
        if (deflectInput)
            characterDefenseController.Deflect();
    }

    //

    public void SetMoveForward(bool state)
    {
        forwardInput = state;
    }

    public void SetMoveBackward(bool state)
    {
        backwardInput = state;
    }

    public void SetMoveRight(bool state)
    {
        rightInput = state;
    }

    public void SetMoveLeft(bool state)
    {
        leftInput = state;
    }

    public void SetSprint(bool state)
    {
        sprintInput = state;
    }

    public void SetDash(bool state)
    {
        dashInput = state;
    }

    public void SetToggleFly(bool state)
    {
        toggleFlyInput = state;
    }

    public void SetJump(bool state)
    {
        jumpInput = state;
    }

    public void SetFlyUp(bool state)
    {
        flyUpInput = state;
    }

    public void SetFlyDown(bool state)
    {
        flyDownInput = state;
    }

    public void SetNormalAttack(bool state)
    {
        normalAttackInput = state;
    }

    public void SetStrikeAttack(bool state)
    {
        strikeAttackInput = state;
    }

    public void SetBlock(bool state)
    {
        blockInput = state;
    }

    public void SetDeflect(bool state)
    {
        deflectInput = state;
    }

    public void SetRotation(Vector3 direction)
    {
        characterMovementController.Rotate(direction);
    }

}
