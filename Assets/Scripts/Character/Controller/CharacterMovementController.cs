using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterStatsData characterStatsData;
    private CharacterStatesData characterStatesData;
    private CharacterObjectService characterObjectService;
    private CharacterObjectsData characterObjectsData;
    private CharacterAnimationService characterAnimationController;
    private CharacterHurtBoxService characterHurtBoxService;
    private CharacterHitBoxesEvents characterHitBoxesEvents;

    // [Event]
    public event Action endFlying;
    public event Action onDashStart;
    public event Action<float> onDashCooldownStart;
    public event Action onDashCooldownEnd;

    private void Start()
    {
        if (characterStatsData == null) characterStatsData = GetComponent<CharacterStatsData>();
        if (characterStatesData == null) characterStatesData = GetComponent<CharacterStatesData>();
        if (characterObjectService == null) characterObjectService = GetComponent<CharacterObjectService>();
        if (characterObjectsData == null) characterObjectsData = GetComponent<CharacterObjectsData>();
        if (characterAnimationController == null) characterAnimationController = GetComponent<CharacterAnimationService>();
        if (characterHurtBoxService == null) characterHurtBoxService = characterObjectsData.characterHurtBox.GetComponent<CharacterHurtBoxService>();
        characterHitBoxesEvents = characterObjectsData.characterMesh.GetComponent<CharacterHitBoxesEvents>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyRotation();
        UpdateAnimations();
    }

    private void ApplyRotation()
    {
        if (characterStatesData.knockAwayFlag || characterStatesData.deadFlag) return;

        Vector3 lookDir = characterStatesData.lookInput;

        if (characterStatesData.fastFlyFlag && !characterStatesData.upperActionFlag && !characterObjectService.IsGrounded)
        {
            characterObjectService.FastFlyingRotate(lookDir);
            characterObjectService.ResizeCollider(true);
            characterHurtBoxService.RotateLocal(new Vector3(90, 0, 0));
            characterHitBoxesEvents.EmitStartFlyAttack();
        }
        else
        {
            lookDir.y = 0;
            characterObjectService.RotateToDirection(lookDir);
            characterObjectService.ResizeCollider(false);
            characterHurtBoxService.RotateLocal(new Vector3(0, 0, 0));
            characterHitBoxesEvents.EmitEndFlyAttack();
        }
    }

    private void HandleMovement()
    {

        // Auto-landing: if we touch the ground while in fly mode, turn it off.
        if (characterStatesData.flyFlag && characterObjectService.IsGrounded)
        {
            SetFlyEnd();
        }

        if (characterStatesData.knockAwayFlag && characterStatesData.fallFlag && characterObjectService.IsGrounded && characterStatesData.currentEndurance <= 0)
        {
            SetFlyEnd();
        }

        if (characterStatesData.knockAwayFlag) return;

        Vector3 moveDirection = GetCurrentMoveDirection();
        characterStatesData.controlledMoveDirection = moveDirection;

        // Activate fast fly flag: Flying + Sprinting + Only Forward input + NOT Blocking
        characterStatesData.fastFlyFlag = characterStatesData.flyFlag &&
                                         characterStatesData.sprintFlag &&
                                         characterStatesData.inputAxes.y > 0 &&
                                         Mathf.Abs(characterStatesData.inputAxes.x) < 0.1f &&
                                         !characterStatesData.blockFlag;

        // Skip normal movement if dashing to prevent force accumulation
        if (characterStatesData.DashFlag) return;

        // Prevent movement during body actions (like strike attacks)
        if (characterStatesData.bodyActionFlag)
            return;

        float currentSpeed = 0;

        if (characterStatesData.flyFlag)
        {
            if (characterStatesData.sprintFlag)
            {
                if (characterStatesData.fastFlyFlag && !characterStatesData.upperActionFlag)
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

            if (!characterObjectService.IsGrounded)
            {
                characterStatesData.fallFlag = characterObjectService.Velocity.y < 0;
            }
            else
            {
                characterStatesData.fallFlag = false;
            }
        }

        if (moveDirection.sqrMagnitude > 0)
        {
            characterObjectService.Move(moveDirection, currentSpeed);
        }

        characterStatesData.controlledMoveSpeed = currentSpeed;

        characterObjectService.ToggleGravity(!characterStatesData.flyFlag);
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

        if (characterStatesData.fastFlyFlag && !characterStatesData.upperActionFlag)
            moveDirection = characterObjectsData.characterObject.transform.forward;

        return moveDirection.normalized;
    }

    private void UpdateAnimations()
    {
        if (characterAnimationController == null) return;

        CharacterBodyAnimation nextAnim = DetermineBodyAnimation();
        if (nextAnim != characterStatesData.currentBodyAnimation)
        {
            characterStatesData.currentBodyAnimation = nextAnim;
            characterAnimationController.PlayBodyAnimation(characterStatesData.currentBodyAnimation);
        }
    }

    public void ForceRefreshBodyAnimation()
    {
        characterStatesData.currentBodyAnimation = (CharacterBodyAnimation)(-1); // Use an invalid value to force update
        UpdateAnimations();
    }

    private CharacterBodyAnimation DetermineBodyAnimation()
    {
        if (characterStatesData.bodyActionFlag)
            return characterStatesData.currentBodyAnimation;

        if (characterStatesData.flyFlag)
        {
            if (characterStatesData.upperActionFlag) return CharacterBodyAnimation.fly_forward;

            if (characterStatesData.fastFlyFlag) return CharacterBodyAnimation.fast_fly;

            // New logic: Check for vertical movement animations if no significant horizontal input
            if (characterStatesData.inputAxes.sqrMagnitude < 0.01f)
            {
                if (characterStatesData.flyUpFlag) return CharacterBodyAnimation.jump;
                if (characterStatesData.flyDownFlag) return CharacterBodyAnimation.fall;
                return CharacterBodyAnimation.fly_idle;
            }

            // Existing logic for horizontal movement
            // Prioritize vertical axis for forward/backward, horizontal for strafing
            if (Mathf.Abs(characterStatesData.inputAxes.y) >= Mathf.Abs(characterStatesData.inputAxes.x))
            {
                return characterStatesData.inputAxes.y > 0 ? CharacterBodyAnimation.fly_forward : CharacterBodyAnimation.fly_backward;
            }
            else
            {
                return characterStatesData.inputAxes.x > 0 ? CharacterBodyAnimation.fly_right : CharacterBodyAnimation.fly_left;
            }
        }
        else
        {
            // If not flying and not on the ground, play jump or fall
            if (!characterObjectService.IsGrounded)
            {
                return characterObjectService.Velocity.y > 0 ? CharacterBodyAnimation.jump : CharacterBodyAnimation.fall;
            }

            if (characterStatesData.inputAxes.sqrMagnitude < 0.01f) return CharacterBodyAnimation.ground_idle;

            bool isSprinting = characterStatesData.sprintFlag;

            if (Mathf.Abs(characterStatesData.inputAxes.y) >= Mathf.Abs(characterStatesData.inputAxes.x))
            {
                if (characterStatesData.inputAxes.y > 0)
                    return isSprinting ? CharacterBodyAnimation.sprint_forward : CharacterBodyAnimation.walking_forward;
                else
                    return isSprinting ? CharacterBodyAnimation.sprint_backward : CharacterBodyAnimation.walking_backward;
            }
            else
            {
                if (characterStatesData.inputAxes.x > 0)
                    return isSprinting ? CharacterBodyAnimation.sprint_right : CharacterBodyAnimation.walking_right;
                else
                    return isSprinting ? CharacterBodyAnimation.sprint_left : CharacterBodyAnimation.walking_left;
            }
        }
    }

    private IEnumerator DashCoroutine()
    {
        characterStatesData.DashFlag = true;

        // Calculate dash direction: prioritizing actual movement direction (velocity)
        Vector3 currentVelocity = characterObjectService.Velocity;
        Vector3 dashDirection = currentVelocity;

        // If not moving significantly, fall back to the input-based direction
        if (dashDirection.magnitude < 0.1f)
        {
            dashDirection = characterStatesData.controlledMoveDirection;
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

        onDashStart?.Invoke();

        yield return new WaitForSeconds(characterStatsData.dashDuration);

        onDashCooldownStart?.Invoke(characterStatsData.dashCooldown);

        characterStatesData.DashFlag = false;
        characterStatesData.dashCooldownFlag = true;

        yield return new WaitForSeconds(characterStatsData.dashCooldown);

        onDashCooldownEnd?.Invoke();

        characterStatesData.dashCooldownFlag = false;
    }

    // [Control methods]
    public void Move(Vector2 direction)
    {
        characterStatesData.inputAxes = direction;
        characterStatesData.moveFlag = direction.sqrMagnitude > 0;
    }

    public void Jump()
    {
        if (!characterStatesData.flyFlag && characterObjectService.IsGrounded && !characterStatesData.bodyActionFlag && characterStatesData.currentProcessAction == CharacterProcessAction.none)
        {
            characterObjectService.SetVerticalVelocity(characterStatsData.jumpForce);
        }
    }

    public void SetSprint(bool status)
    {
        if (status == characterStatesData.sprintFlag) return;

        characterStatesData.sprintFlag = status;
    }

    public void Dash()
    {
        if (characterStatesData.DashFlag || characterStatesData.dashCooldownFlag)
            return;
        
        if (characterStatesData.bodyActionFlag && characterStatesData.currentProcessAction != CharacterProcessAction.strike_attack)
            return;

        StartCoroutine(DashCoroutine());
    }

    public void SetFly(bool status)
    {
        if (status == characterStatesData.flyFlag) return;

        characterStatesData.flyFlag = status;
    }

    public void SetFlyEnd()
    {
        characterStatesData.flyFlag = false;
        characterObjectService.ToggleGravity(true);

        endFlying?.Invoke();

        if (!characterObjectService.IsGrounded)
        {
            characterStatesData.fallFlag = true;
        }
        else
        {
            characterStatesData.fallFlag = false;
        }
    }

    public void SetFlyUp(bool status)
    {
        if (status == characterStatesData.flyUpFlag) return;

        characterStatesData.flyUpFlag = status;
    }

    public void SetFlyDown(bool status)
    {
        if (status == characterStatesData.flyDownFlag) return;

        characterStatesData.flyDownFlag = status;
    }

    public void Rotate(Vector3 lookInput)
    {
        characterStatesData.lookInput = lookInput;
    }

}
