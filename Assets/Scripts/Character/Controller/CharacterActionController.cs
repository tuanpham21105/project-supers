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

    private bool sprintInput;
    private bool dashInput;
    private bool toggleFlyInput;
    private bool jumpInput;
    private bool flyUpInput;
    private bool flyDownInput;
    private bool normalAttackInput;
    private bool strikeAttackInput;
    private bool blockInput;
    private bool deflectInput;
    private bool forwardInput;
    private bool backwardInput;
    private bool leftInput;
    private bool rightInput;
    
    public event Action onFlyingInterrupted;

    void Start()
    {
        characterMovementController = GetComponent<CharacterMovementController>();
        characterAttackController = GetComponent<CharacterAttackController>();
        characterDefenseController = GetComponent<CharacterDefenseController>();

        characterMovementController.endFlying += HandleFlyingInterrupted;
    }

    void FixedUpdate()
    {
        if (!MatchManager.instance.IsPlayerHost()) return;

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
    }

    void HandleFlyingInterrupted()
    {
        onFlyingInterrupted?.Invoke();
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
