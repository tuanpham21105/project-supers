using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectService : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterController characterController;
    private CharacterStatesData characterStatesData;

    // [Constant]
    [Header("Constant")]
    [SerializeField] private bool gravity;
    [SerializeField] private float impactDecaySpeed = 1f;
    [SerializeField] private float fastFlyRotationSpeed = 10f;
    [SerializeField] private float normalRotationSpeed = 360f;

    // [Event]
    public event Action OnImpactForceDecayed;
    public event Action OnImpactCollision;

    public bool IsGrounded => characterController.isGrounded;
    public Transform CharacterTransform => transform;
    public Vector3 Velocity => characterController.velocity;
    public Vector3 ImpactForceDirection => characterStatesData.impactForce.normalized;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterStatesData = GetComponent<CharacterStatesData>();
    }

    void FixedUpdate()
    {
        Vector3 startPosition = transform.position;
        Vector3 combinedMove = Vector3.zero;

        // 1. Handle Horizontal Movement
        combinedMove += characterStatesData.horizontalMove;
        characterStatesData.horizontalMove = Vector3.zero; // Reset after consuming

        // 2. Handle Gravity & Vertical Velocity
        if (gravity)
        {
            if (characterController.isGrounded && characterStatesData.verticalVelocity < 0)
            {
                // characterStatesData.verticalVelocity = -2f;
                characterStatesData.verticalVelocity = 0f;
            }
            characterStatesData.verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }
        combinedMove.y += characterStatesData.verticalVelocity;

        // 3. Handle Impact Forces
        // Decay the impact force over time
        characterStatesData.impactForce = Vector3.Lerp(characterStatesData.impactForce, Vector3.zero, impactDecaySpeed * Time.fixedDeltaTime);

        bool impactAppliedThisFrame = false;
        if (characterStatesData.impactForce.magnitude > 10f)
        {
            combinedMove += characterStatesData.impactForce;
            characterStatesData.isImpactActive = true;
            impactAppliedThisFrame = true;
        }
        else if (characterStatesData.isImpactActive)
        {
            characterStatesData.isImpactActive = false;
            characterStatesData.impactForce = Vector3.zero;
            OnImpactForceDecayed?.Invoke();
        }

        // 4. Handle Dash
        if (characterStatesData.dashTimer > 0)
        {
            combinedMove += characterStatesData.dashForce;
            characterStatesData.dashTimer -= Time.fixedDeltaTime;
            if (characterStatesData.dashTimer <= 0) characterStatesData.dashForce = Vector3.zero;
        }

        // Apply everything in one call per tick
        characterController.Move(combinedMove * Time.fixedDeltaTime);

        characterStatesData.currentMoveDirection = combinedMove.normalized;
        characterStatesData.currentSqrMoveSpeed = combinedMove.sqrMagnitude;

        if (impactAppliedThisFrame)
        {
            float distanceMoved = Vector3.Distance(transform.position, startPosition);
            float expectedDistance = combinedMove.magnitude * Time.fixedDeltaTime;

            if (expectedDistance > 0.01f && distanceMoved < 0.001f)
            {
                characterStatesData.impactForce = Vector3.zero;
                characterStatesData.isImpactActive = false;
                OnImpactCollision?.Invoke();
            }
        }
    }

    public void ToggleGravity(bool status)
    {
        gravity = status;
        if (!status) characterStatesData.verticalVelocity = 0;
    }

    public void Move(Vector3 direction, float moveSpeed)
    {
        characterStatesData.horizontalMove = direction * moveSpeed;
    }

    public void SetVerticalVelocity(float velocity)
    {
        characterStatesData.verticalVelocity = velocity;
    }

    public void ApplyForce(Vector3 force)
    {
        characterStatesData.impactForce += force;
    }

    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        transform.forward = direction;
    }

    public void SetUpDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        transform.up = direction;
    }

    public void RotateToDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, normalRotationSpeed * Time.deltaTime);
    }

    public void FastFlyingRotate(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Vector3 forward = direction.normalized;
        Vector3 up = Vector3.up;

        // Avoid gimbal lock when flying perfectly vertical
        if (Mathf.Abs(Vector3.Dot(forward, up)) > 0.99f)
        {
            up = Vector3.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(forward, up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, fastFlyRotationSpeed * Time.fixedDeltaTime);
    }

    public void Dash(Vector3 direction, float force, float duration)
    {
        characterStatesData.dashForce = direction.normalized * force;
        characterStatesData.dashTimer = duration;
    }

    public bool IsPointFront(Vector3 point) {
        return Vector3.Dot(point, transform.forward) <= 0;
    }

    public bool IsFaceUp()
    {
        return !IsPointFront(Vector3.up);
    }

    public void ResizeCollider(bool isFastFly)
    {
        if (isFastFly)
        {
            GetComponent<CharacterController>().height = GetComponent<CharacterController>().radius;
        }
        else
        {
            GetComponent<CharacterController>().height = 2f;
        }
    }
}
