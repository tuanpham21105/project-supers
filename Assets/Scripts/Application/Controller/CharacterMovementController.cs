using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    [SerializeField] private CharacterStatsData characterStatsData;
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterObjectService characterObjectService;
    [SerializeField] private CharacterObjectsData characterObjectsData;

    private void Start()
    {
        // Initialize rotation states from current transform orientations to prevent jumping at start
        characterStatesData.horizontalRotation = characterObjectService.CharacterTransform.eulerAngles.y;
    }

    private void Update()
    {
        ApplyRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void ApplyRotation()
    {
        Vector3 lookDir = characterStatesData.lookInput;

        if (characterStatesData.fastFlyFlag)
        {
            characterObjectService.FastFlyingRotate(lookDir);
        }
        else
        {
            lookDir.y = 0;
            characterObjectService.RotateToDirection(lookDir);
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetCurrentMoveDirection();
        characterStatesData.direction = moveDirection;

        // Activate fast fly flag: Flying + Sprinting + Only Forward input
        characterStatesData.fastFlyFlag = characterStatesData.flyFlag && 
                                         characterStatesData.sprintFlag && 
                                         characterStatesData.inputAxes.y > 0 && 
                                         Mathf.Abs(characterStatesData.inputAxes.x) < 0.1f;

        // Skip normal movement if dashing to prevent force accumulation
        if (characterStatesData.dashFlag) return;

        float currentSpeed = 0;

        if (characterStatesData.flyFlag)
        {
            if (characterStatesData.sprintFlag)
            {
                if (characterStatesData.fastFlyFlag)
                    currentSpeed = characterStatsData.flySpeed + characterStatsData.flySprintAdditionalSpeed;
                else
                    currentSpeed = characterStatsData.flySpeed + characterStatsData.sprintAdditionalSpeed;
            }
            else
            {
                currentSpeed = characterStatsData.flySpeed;
            }
        }
        else
        {
            currentSpeed = characterStatsData.moveSpeed + (characterStatesData.sprintFlag ? characterStatsData.sprintAdditionalSpeed : 0);
        }

        if (moveDirection.sqrMagnitude > 0)
        {
            characterObjectService.Move(moveDirection, currentSpeed);
        }

        // Auto-landing: if we touch the ground while in fly mode, turn it off.
        if (characterStatesData.flyFlag && characterObjectService.IsGrounded)
        {
            ToggleFly(false);
        }
    }

    private Vector3 GetCurrentMoveDirection()
    {
        Vector3 moveDirection = Vector3.zero;
        Vector3 forward = characterObjectService.CharacterTransform.forward;
        Vector3 right = characterObjectService.CharacterTransform.right;

        Vector3 forwardPlanar = forward;
        Vector3 rightPlanar = right;
        forwardPlanar.y = 0;
        rightPlanar.y = 0;
        forwardPlanar.Normalize();
        rightPlanar.Normalize();

        moveDirection = (forwardPlanar * characterStatesData.inputAxes.y + rightPlanar * characterStatesData.inputAxes.x);

        if (characterStatesData.flyFlag)
        {
            // Fly without sprint: floating, can fly up and fly down
            if (characterStatesData.flyUpFlag) moveDirection += Vector3.up;
            if (characterStatesData.flyDownFlag) moveDirection += Vector3.down;
        }

        if (characterStatesData.fastFlyFlag)
            moveDirection = characterObjectsData.characterObject.transform.up;

        return moveDirection.normalized;
    }

    public void Move(Vector2 direction)
    {
        characterStatesData.inputAxes = direction;
        characterStatesData.moveFlag = direction.sqrMagnitude > 0;
    }

    public void Jump()
    {
        // Jump only work when on ground and not flying
        if (!characterStatesData.flyFlag && characterObjectService.IsGrounded)
        {
            characterObjectService.SetVerticalVelocity(characterStatsData.jumpForce);
        }
    }

    public void Sprint(bool status)
    {
        characterStatesData.sprintFlag = status;
    }

    public void Dash()
    {
        if (characterStatesData.dashFlag || characterStatesData.dashCooldownFlag) return;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        characterStatesData.dashFlag = true;
        
        // Calculate dash direction: prioritizing actual movement direction (velocity)
        Vector3 currentVelocity = characterObjectService.Velocity;
        Vector3 dashDirection = currentVelocity;

        // If not moving significantly, fall back to the input-based direction
        if (dashDirection.magnitude < 0.1f)
        {
            dashDirection = characterStatesData.direction;
        }

        // If still no direction (no movement and no input), default to character's forward
        if (dashDirection.sqrMagnitude < 0.001f)
        {
            dashDirection = characterObjectService.CharacterTransform.forward;
        }

        // If grounded, clear any downward vertical velocity (like the gravity stabilizer) 
        // to ensure the dash stays horizontal unless there's an intentional upward jump/fly movement.
        if (characterObjectService.IsGrounded && dashDirection.y < 0)
        {
            dashDirection.y = 0;
        }

        characterObjectService.Dash(dashDirection.normalized, characterStatsData.dashForce, characterStatsData.dashDuration);

        yield return new WaitForSeconds(characterStatsData.dashDuration);

        characterStatesData.dashFlag = false;
        characterStatesData.dashCooldownFlag = true;

        yield return new WaitForSeconds(characterStatsData.dashCooldown);

        characterStatesData.dashCooldownFlag = false;
    }

    public void ToggleFly(bool status)
    {
        characterStatesData.flyFlag = status;
        characterObjectService.ToggleGravity(!status);
    }

    public void FlyUp(bool status)
    {
        characterStatesData.flyUpFlag = status;
    }

    public void FlyDown(bool status)
    {
        characterStatesData.flyDownFlag = status;
    }

    public void Rotate(Vector3 lookInput)
    {
        characterStatesData.lookInput = lookInput;
    }
}
