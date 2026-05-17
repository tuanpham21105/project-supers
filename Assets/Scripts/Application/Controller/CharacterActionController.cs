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

    private bool forwardMove;
    private bool backwardMove;
    private bool leftMove;
    private bool rightMove;
    
    public event Action onFlyingInterrupted;

    void Start()
    {
        characterMovementController = GetComponent<CharacterMovementController>();
        characterAttackController = GetComponent<CharacterAttackController>();
        characterDefenseController = GetComponent<CharacterDefenseController>();

        characterMovementController.endFlying += HandleFlyingInterrupted;
    }

    void Update()
    {
        MoveDirection();
    }

    void OnDestroy()
    {
        characterMovementController.endFlying -= HandleFlyingInterrupted;
    }

    void HandleFlyingInterrupted()
    {
        onFlyingInterrupted?.Invoke();
    }

    public void MoveForward(bool state)
    {
        forwardMove = state;
    }

    public void MoveBackward(bool state)
    {
        backwardMove = state;
    }

    public void MoveRight(bool state)
    {
        rightMove = state;
    }

    public void MoveLeft(bool state)
    {
        leftMove = state;
    }

    private void MoveDirection()
    {
        float x = 0;
        float y = 0;

        if (forwardMove) y += 1;
        if (backwardMove) y -= 1;
        if (leftMove) x -= 1;
        if (rightMove) x += 1;

        Vector2 currentMoveInput = new Vector2(x, y).normalized;

        characterMovementController.Move(currentMoveInput);
    }

    public void Sprint(bool state)
    {
        characterMovementController.SetSprint(state);
    }

    public void Dash(bool state)
    {
        if (state)
            characterMovementController.Dash();
    }

    public void ToggleFly(bool state)
    {
        characterMovementController.SetFly(state);
    }

    public void Jump(bool state)
    {
        if (state)
            characterMovementController.Jump();
    }

    public void FlyUp(bool state)
    {

        characterMovementController.SetFlyUp(state);
    }

    public void FlyDown(bool state)
    {

        characterMovementController.SetFlyDown(state);
    }

    public void NormalAttack(bool state)
    {
        if (state)
            characterAttackController.StartNormalAttack();
    }

    public void StrikeAttack(bool state)
    {
        if (state)
            characterAttackController.StartStrikeAttack();
    }

    public void Block(bool state)
    {
        characterDefenseController.Block(state);
    }

    public void Deflect(bool state)
    {
        if (state)
            characterDefenseController.Deflect();
    }

    public void Rotation(Vector3 direction)
    {
        characterMovementController.Rotate(direction);
    }

}
