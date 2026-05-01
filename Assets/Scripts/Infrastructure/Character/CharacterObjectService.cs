using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectService : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    [SerializeField] private bool gravity;
    [SerializeField] private float impactDecaySpeed = 1f;
    [SerializeField] private float fastFlyRotationSpeed = 10f;
    [SerializeField] private float normalRotationSpeed = 360f;

    public bool IsGrounded => characterController.isGrounded;
    public Transform CharacterTransform => transform;
    public Vector3 Velocity => characterController.velocity;
    public Vector3 ImpactForceDirection => _impactForce.normalized;

    public event Action OnImpactForceDecayed;
    public event Action OnImpactCollision;

    private float _verticalVelocity;
    private Vector3 _impactForce;
    private Vector3 _dashForce;
    private float _dashTimer;
    private Vector3 _horizontalMove;
    private bool _isImpactActive;
    [SerializeField] private Vector3 currentMoveDirection;
    [SerializeField] private float currentSqrMoveSpeed;

    public Vector3 GetMoveDirection() => currentMoveDirection;
    public float GetSqrMoveSpeed() => currentSqrMoveSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Vector3 startPosition = transform.position;
        Vector3 combinedMove = Vector3.zero;

        // 1. Handle Horizontal Movement
        combinedMove += _horizontalMove;
        _horizontalMove = Vector3.zero; // Reset after consuming

        // 2. Handle Gravity & Vertical Velocity
        if (gravity)
        {
            if (characterController.isGrounded && _verticalVelocity < 0)
            {
                // _verticalVelocity = -2f;
                _verticalVelocity = 0f;
            }
            _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }
        combinedMove.y += _verticalVelocity;

        // 3. Handle Impact Forces
        // Decay the impact force over time
        _impactForce = Vector3.Lerp(_impactForce, Vector3.zero, impactDecaySpeed * Time.fixedDeltaTime);

        bool impactAppliedThisFrame = false;
        if (_impactForce.magnitude > 10f)
        {
            combinedMove += _impactForce;
            _isImpactActive = true;
            impactAppliedThisFrame = true;
        }
        else if (_isImpactActive)
        {
            _isImpactActive = false;
            _impactForce = Vector3.zero;
            OnImpactForceDecayed?.Invoke();
        }

        // 4. Handle Dash
        if (_dashTimer > 0)
        {
            combinedMove += _dashForce;
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0) _dashForce = Vector3.zero;
        }

        // Apply everything in one call per tick
        characterController.Move(combinedMove * Time.fixedDeltaTime);

        currentMoveDirection = combinedMove.normalized;
        currentSqrMoveSpeed = combinedMove.sqrMagnitude;

        if (impactAppliedThisFrame)
        {
            float distanceMoved = Vector3.Distance(transform.position, startPosition);
            float expectedDistance = combinedMove.magnitude * Time.fixedDeltaTime;

            if (expectedDistance > 0.01f && distanceMoved < 0.001f)
            {
                _impactForce = Vector3.zero;
                _isImpactActive = false;
                OnImpactCollision?.Invoke();
            }
        }
    }

    public void ToggleGravity(bool status)
    {
        gravity = status;
        if (!status) _verticalVelocity = 0;
    }

    public void Move(Vector3 direction, float moveSpeed)
    {
        _horizontalMove = direction * moveSpeed;
    }

    public void SetVerticalVelocity(float velocity)
    {
        _verticalVelocity = velocity;
    }

    public void ApplyForce(Vector3 force)
    {
        _impactForce += force;
    }

    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        transform.forward = direction;
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
        _dashForce = direction.normalized * force;
        _dashTimer = duration;
    }

    public bool IsPointFront(Vector3 point) {
        return Vector3.Dot(point, transform.forward) <= 0;
    }
}
